import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')
import cv2
import sklearn
import pickle
import os
import json
from sklearn.naive_bayes import GaussianNB
import numpy as np
from features import extra_features

def get_named_contours():
    for filename in os.listdir('shape_training'):
        if filename.split('.')[-1] == 'pickle':
            shapes = pickle.load(open(os.path.join('shape_training', filename)))
            named_indices = json.load(open(os.path.join('shape_training', filename.split('.')[0] + '.json')))
            for name, index in named_indices.iteritems():
                yield (name, shapes[index])

def create_classifier_nb(feature_vecs, names):
    gnb = GaussianNB()
    gnb = gnb.fit(feature_vecs, names)
    def classify(vec):
        cls = gnb.predict(vec)
        score = 0
        return cls, score
    return classify

def create_classifier(feature_vecs, names):
    # print feature_vecs
    bounds = compute_bounds(feature_vecs)
    # print 'bounds', bounds
    feature_vecs_normalized = [normalize_features(ft, bounds) for ft in feature_vecs]    
    def match_score(vec1, vec2):
        # print vec1
        # print vec2
        # print
        return sum([squared_diff(f1, f2) for f1, f2 in zip(vec1, vec2)])
    
    def classify(item):
        item_normalized = normalize_features(item, bounds)
        # print item_normalized
        min_idx = min(range(len(names)), key=lambda i: match_score(item_normalized, feature_vecs_normalized[i]))
        score = match_score(item_normalized, feature_vecs_normalized[min_idx])
        return names[min_idx], score
    
    return classify

def points_to_np(points):
    return np.array([np.array(n) for n in points]).astype(np.float32)

def features_from_contour(contour):
    # print contour
    moments = cv2.moments(contour)
    # print cv2.HuMoments(contour)
    feats = []
    feats += list([x[0] for x in cv2.HuMoments(moments)])[:-1]
    # print feats[0]
    feats.append(cv2.contourArea(contour))
    feats.append(cv2.arcLength(contour, True))
    
    points = points_to_np([ps[0] for ps in contour])
    # ((center_x,center_y),(w,h),rotation) = cv2.minAreaRect(points)    
    # feats += [w, h, w*1.0/h]
    
    feats += extra_features(contour)
    
    feats.append(len(contour))
    return feats

def compute_bounds(feature_vectors):
    mins = list(feature_vectors[0])
    maxes = list(feature_vectors[0])
    for vec in feature_vectors:
        for i, val in enumerate(vec):
            mins[i] = min(mins[i], val)
            maxes[i] = max(maxes[i], val)
    return list(zip(mins, maxes))

def normalize_features(vec, bounds):
    return [(x - _min) / (_max - _min + 0.00001) for x, (_min, _max) in zip(vec, bounds)]

def squared_diff(a, b):
    return (a-b)**2
    # d = abs(a-b) / ((a + b)/2.0 + 0.000001)
    # return d ** 2
