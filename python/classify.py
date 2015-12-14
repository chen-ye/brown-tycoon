import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')
import cv2

def features_from_contour(contour):
    feats = []
    feats += list(cv2.HuMoments(contour))
    feats.append(cv2.contourArea(contour))
    feats.append(cv2.arcLength(contour, True))
    feats.append(len(contour))
    return feats

def compute_bounds(feature_vectors):
    mins = feature_vectors[0]
    maxes = feature_vectors[0]
    for vec in feature_vectors:
        for i, val in enumerate(vec):
            mins[i] = min(mins[i], val)
            maxes[i] - max(maxes[i], val)
    return list(zip(mins, maxes))

def normalize_features(vec, bounds):
    return [(x - _min) / (_max - _min + 0.00001) for x, (_min, _max) in zip(vec, bounds)]

def squared_diff(a, b):
    d = abs(a-b) / (a + b + 0.000001)
    return d ** 2

def classify(item, named_features):
    bounds = compute_bounds(named_features.values())
    named_features_normalized = {name: normalize_features(features, bounds) for name, features in named_features.iteritems()}
    item_normalized = normalize_features(item, bounds)
    def match_score(vec1, vec2):
        return sum([squared_diff(f1, f2) for f1, f2 in zip(vec1, vec2)])
    named_match_scores = {name: match_score(item_normalized, named_features_normalized[name]) for name in named_features_normalized.iterkeys()}
    return min(named_match_scores.iterkeys(), key=lambda name: named_match_scores[name]), min(named_match_scores.itervalues())
