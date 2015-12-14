import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')

import cv2
import numpy as np
import os
import pickle
import sys

CONTOUR_SIMPLIFICATION = 60 # lower is stronger
MIN_SHAPE_SIZE = 70
THRESH_RADIUS = 31
THRESH_CONSTANT = 10
MAX_EDGES = 10
BLUR_RADIUS = 3
IMAGE_SIZE = (640, 480)
MAX_CONTOURS_PER_STAGE = 10
MAX_CONTOURS_TO_STORE = 20
MAX_CONTOUR_DISTANCE_BETWEEN_FRAMES = 0.2

# ROI

def match_items(set1, set2, diff_func):
    # set1, set2 should be lists not sets, lol
    indices_takens_from_set1 = set()
    indices_takens_from_set2 = set()
    
    scores = {}
    for i1, o1 in enumerate(set1):
        for i2, o2 in enumerate(set2):
            d = diff_func(o1, o2)
            if d is not None:
                scores[(i1,i2)] = d
    
    #if len(scores):
    #    print min(scores.values())
    
    for i1, i2 in sorted(scores.keys(), key=lambda ids: scores[ids]):
        if i1 not in indices_takens_from_set1 and i2 not in indices_takens_from_set2:
            yield (set1[i1], set2[i2])
            indices_takens_from_set1.add(i1)
            indices_takens_from_set2.add(i2)
    
    for i1, o1 in enumerate(set1):
        if i1 not in indices_takens_from_set1:
            yield o1, None
    
    for i2, o2 in enumerate(set2):
        if i2 not in indices_takens_from_set2:
            yield None, o2

def contour_distance(c1, c2):
    img_w, img_h = IMAGE_SIZE
    img_diag = (img_w**2 + img_h**2)**0.5
    
    c1_x, c1_y = contour_center(c1)
    c2_x, c2_y = contour_center(c2)
    pos_diff = ( (c1_x - c2_x) ** 2 + (c1_y - c2_y) ** 2 ) ** 0.5 / img_diag
    
    return pos_diff

def contour_center(contour):
    # TODO: make this mathematically correct
    xsum = 0.0
    ysum = 0.0
    # print contour
    for p in contour:
        x,y = p[0]
        xsum += x
        ysum += y
    return (xsum / len(contour), ysum / len(contour))

def contour_difference(c1, c2, c2_age=1, pos_weight=1):        
    pos_diff = contour_distance(c1, c2)
    if pos_diff > MAX_CONTOUR_DISTANCE_BETWEEN_FRAMES and pos_weight > 0:
        return None
    
    shape_diff = cv2.matchShapes(c1, c2, cv2.cv.CV_CONTOURS_MATCH_I1, 0.0)
    # return shape_diff
    
    size1 = cv2.contourArea(c1)
    size2 = cv2.contourArea(c2)
    size_diff = abs(size1 - size2) / ((size1 + size2) / 2)
    
    point_count_diff = abs(len(c1) - len(c2)) / ((len(c1) + len(c2)) / 2.0)
    
    SHAPE_WEIGHT = 20
    SIZE_WEIGHT = 10
    POINT_COUNT_WEIGHT = 10
    AGE_EXP = 1 # >= 0
    
    return (pos_diff*pos_weight + shape_diff*SHAPE_WEIGHT + size_diff*SIZE_WEIGHT + point_count_diff * POINT_COUNT_WEIGHT) * c2_age ** AGE_EXP

class ContourClassifier(object):
    def __init__(self, data_path, names):
        contours = pickle.loads(open(data_path).read())
        self.named_contours = {name: contours[i] for name, i in names.iteritems()}
    
    def classify(self, contour):
        names = self.named_contours.keys()
        return min(names, key=lambda name: contour_difference(contour, self.named_contours[name], pos_weight=0))

class ContourMatcher(object):
    def __init__(self, classifier):
        self.contours_dict = {}
        self.contour_ages = {}
        self.last_id = 0
        self.classes_by_id = {}
        self.classifier = classifier
    
    def assign_id(self):
        self.last_id += 1
        return self.last_id
    
    def run_frame(self, new_contours):
        new_contours.sort(key=lambda c: cv2.contourArea(c), reverse=True)
        new_contours = new_contours[:min(len(new_contours), MAX_CONTOURS_PER_STAGE)]
        
        new_ages = {id: age+1 for id, age in self.contour_ages.iteritems()}
        new_contours_dict = dict(self.contours_dict)
        
        def diff(old_contour_id, new_contour):
            return contour_difference(self.contours_dict[old_contour_id], new_contour, new_ages[old_contour_id])
        
        for old_contour_id, new_contour in match_items(self.contours_dict.keys(), new_contours, diff):
            if old_contour_id is not None and new_contour is not None:
                new_contours_dict[old_contour_id] = new_contour
                new_ages[old_contour_id] = 0
            elif old_contour_id is None and new_contour is not None:
                id = self.assign_id()
                new_contours_dict[id] = new_contour
                new_ages[id] = 0
                self.classes_by_id[id] = self.classifier.classify(new_contour)
        
        # drop old contours:
        contour_ids = sorted(new_contours_dict.keys(), key=lambda id: new_ages[id], reverse=True)
        ids_to_cut = max(0, len(contour_ids) - MAX_CONTOURS_TO_STORE)
        for old_id in contour_ids[:ids_to_cut]:
            del new_contours_dict[old_id]
            del new_ages[old_id]
        
        self.contours_dict = new_contours_dict
        self.contour_ages = new_ages
        return {id: contour for id, contour in new_contours_dict.iteritems() if new_ages[id] == 0}
    
    def best_match_identifier(self, new_contour, old_contours_dict):
        if len(old_contours_dict) == 0: return None
        return min(old_contours_dict.keys(), key=lambda id: contour_difference(new_contour, old_contours_dict[id], self.contour_ages[id]))
    
    def class_of_id(self, id):
        return self.classes_by_id[id]
    
    def dump(self, path):
        open(path, 'w').write(pickle.dumps(self.contours_dict))

def get_contours(frame):
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    # gray = cv2.equalizeHist(gray)
    # cv2.imshow('gray', gray)
    # cv2.waitKey(0)
    # quit()
    
    blurred = cv2.blur(gray, (BLUR_RADIUS, BLUR_RADIUS))
    # edges = cv2.Canny(blurred, 20, 100)
    edges = cv2.adaptiveThreshold(blurred, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, THRESH_RADIUS, THRESH_CONSTANT)
    # cv2.imshow('edges', edges)
    # cv2.waitKey(0)
    retr_external = False
    contours, _ = cv2.findContours(edges, (cv2.RETR_EXTERNAL if retr_external else cv2.RETR_LIST), cv2.CHAIN_APPROX_SIMPLE)
    
    out = []
    
    for contour in contours:
        if cv2.contourArea(contour) > MIN_SHAPE_SIZE:
            contour = cv2.approxPolyDP(contour, cv2.arcLength(contour, True) / CONTOUR_SIMPLIFICATION, True)
            if len(contour) <= MAX_EDGES:
                out.append(contour)
            # cv2.drawContours(frame, [contour], -1, (255,0,0))
    
    return out

def process_image(frame):
    frame = cv2.resize(frame, IMAGE_SIZE)
    
    i = 0
    contours = get_contours(frame)
    id_contours = matcher.run_frame(contours)
    for id, contour in id_contours.iteritems():
        cv2.drawContours(frame, [contour], -1, (255,0,0))
        x,y = contour[0][0]
        cv2.putText(frame, "{0}-{1}".format(id, matcher.class_of_id(id)), (x,y), cv2.FONT_HERSHEY_PLAIN, 0.7, (0, 0, 255))
        i += 1

    # Display the resulting frame
    cv2.imshow('frame', frame)

def get_images():
    p = 'images'
    for name in os.listdir(p):
        if name[-4:] == '.jpg':
            yield cv2.imread(os.path.join(p, name)) 

def photo_loop():
    for image in get_images():
        # cv2.imshow('frame', image)
        process_image(image)
        cv2.waitKey(0)

def video_loop():
    cap = cv2.VideoCapture(1)
    
    while(True):
        # Capture frame-by-frame
        ret, frame = cap.read()
        
        process_image(frame)
        # cv2.waitKey(0)
        
        # Our operations on the frame come here
        
        #if cv2.waitKey(1) & 0xFF == ord('q'):
        #    break
    
    cap.release()
    cv2.destroyAllWindows()

def snap_and_dump():
    cap = cv2.VideoCapture(1)
    
    ret, frame = cap.read()
        
    process_image(frame)
    cv2.waitKey(0)
    
    cap.release()
    cv2.destroyAllWindows()
    
    matcher.dump("shapes.pickle")

"""names = {
    "rect": 4,
    "small_tri": 12,
    "medium_tri": 9,
    "trap": 2,
    "arrowhead": 11,
    "concave": 14
}"""
names = {
    "square": 7,
    "tri": 9,
    "rect": 4,
    "trap": 8,
    "pentagon": 2,
    "small_pent": 6,
    "small_square": 7
}
classifier = ContourClassifier("shapes.pickle", names)
matcher = ContourMatcher(classifier)

if __name__ == '__main__':
    if '--snap-and-dump' in sys.argv:
        snap_and_dump()
    else:
        video_loop()
    # photo_loop()
    # matcher.dump("shapes.pickle")

