# import shape_similarity
import classify

import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')
import cv2

import compute_rotation

from collections import defaultdict

class ShapeDesc(object):
    def __init__(self, contour, screen_size):
        center = [0,0]
        for pt in contour:
            center[0] += pt[0][0]
            center[1] += pt[0][1]
        self.center = [center[0] / len(contour), center[1] / len(contour)]
        # self.token = shape_similarity.contour_to_token(contour)
        self.screen_size = screen_size
        self.screen_diag = (screen_size[0] ** 2 + screen_size[1] ** 2) ** 0.5
        self.features = classify.features_from_contour(contour)
        self.area = cv2.contourArea(contour)
        self.shape_name = None
        self.contour = contour
    
    def dist(self, other):
        dist = ((self.center[0] - other.center[0]) ** 2 + (self.center[1] - other.center[1]) ** 2) ** 0.5
        return dist / self.screen_diag

class Matcher(object):
    def __init__(self, screen_size):
        self.screen_size = screen_size
        self.screen_diag = (self.screen_size[0] ** 2 + self.screen_size[1] ** 2) ** 0.5
        
        self.named_contours = defaultdict(list)
        feature_vecs = []
        output_labels = []
        for name, contour in classify.get_named_contours():
            self.named_contours[name].append(contour)
            feature_vecs.append(classify.features_from_contour(contour))
            output_labels.append(name)
        
        # contours = [contour for _, contour in classify.get_named_contours()]
        # names = [name for name, _ in classify.get_named_contours()]
        # self.classifier = classify.create_contour_classifier(contours, names)
        self.classifier = classify.create_classifier(feature_vecs, output_labels)
        
        self.prev_descriptors = {}
        self.last_id = 0
    
    def closest_prev_shape_id(self, descriptor, name, exclude_ids=set()):
        id_distances = {id: descriptor.dist(other_desc) for id, other_desc in self.prev_descriptors.iteritems() if other_desc.shape_name == name and id not in exclude_ids}
        if len(id_distances) == 0:
            return None, 9999999
        min_id = min(id_distances.keys(), key=lambda id: id_distances[id])
        min_dist = min(id_distances.values())
        return min_id, min_dist
    
    def assign_id(self):
        self.last_id += 1
        return self.last_id
    
    def run_frame(self, contours):
        descriptors = []
        for contour in contours:
            desc = ShapeDesc(contour, self.screen_size)
            name, score = self.classify_descriptor(desc)
            desc.shape_name = name
            descriptors.append(desc)
        descriptors.sort(key=lambda d: self.closest_prev_shape_id(d, d.shape_name)[1]) # sort by shortest distance to match
        
        descriptors_by_id = {}
        ids_consumed = set()
        for desc in descriptors:
            match_id, dist = self.closest_prev_shape_id(desc, desc.shape_name, exclude_ids=ids_consumed)
            if match_id:
                ids_consumed.add(match_id)
                descriptors_by_id[match_id] = desc
            else:
                descriptors_by_id[self.assign_id()] = desc
                
        self.prev_descriptors = descriptors_by_id
        return [(desc.contour, id, desc.shape_name, desc) for id, desc in descriptors_by_id.iteritems()]
        # returns list of (contour, id, name, descriptor)
    
    def classify_descriptor(self, desc): # returns (name, match score)
        # name, affinity = self.classifier(desc.contour)
        name, affinity = self.classifier(desc.features)
        # print name
        return name, affinity
        # return classify.classify(desc.features, self.named_feature_vecs)
    
    def get_rotation(self, name, contour):
        contour2 = self.named_contours[name][0]
        return compute_rotation.compute_rotation(contour, contour2)
    
    def classify_contour(self, contour):
        desc = ShapeDesc(contour, self.screen_size)
        name, affinity = self.classify_descriptor(desc)
        return name
        
