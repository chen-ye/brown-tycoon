import shape_similarity

import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')
import cv2

class ShapeDesc(object):
    def __init__(self, contour, screen_size):
        center = [0,0]
        for pt in contour:
            center[0] += pt[0][0]
            center[1] += pt[0][1]
        self.center = [center[0] / len(contour), center[1] / len(contour)]
        self.token = shape_similarity.contour_to_token(contour)
        self.area = cv2.contourArea(contour)
        self.screen_size = screen_size
        self.screen_diag = (screen_size[0] ** 2 + screen_size[1] ** 2) ** 0.5
        self.shape_name = None
        self.contour = contour
    
    def match_score_ignoring_distance(self, other): # lower is better
        size_diff = abs(self.area - other.area) / (self.area/2.0 + other.area/2.0)
        if size_diff > 1.3:
            return None
        return (0.1 + shape_similarity.token_diff(self.token, other.token)) * (0.1 * size_diff)
    
    def dist(self, other):
        dist = ((self.center[0] - other.center[0]) ** 2 + (self.center[1] - other.center[1]) ** 2) ** 0.5
        return dist / self.screen_diag
    
    """def match_score(self, other):
        # lower is better
        dist = ((self.center[0] - other.center[0]) ** 2, (self.center[1] - other.center[1]) ** 2) ** 0.5
        if dist / self.screen_diag > 0.1:
            return None
        size_diff = abs(self.area - other.area) / (self.area/2.0 + other.area/2.0)
        return (0.1 + shape_similarity.token_diff(self.token, other.token)) * (0.1 + dist / self.screen_diag) * (0.1 + size_diff)"""

class Matcher(object):
    def __init__(self, named_shapes, screen_size):
        self.named_shapes = named_shapes
        self.screen_size = screen_size
        
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
        names = self.named_shapes.keys()
        named_scores = {name: desc.match_score_ignoring_distance(self.named_shapes[name]) for name in names}
        non_null_names = [name for name, score in named_scores.iteritems() if score is not None]
        if len(non_null_names) == 0:
            return None, None
        else:
            name = min(non_null_names, key=lambda name: named_scores[name])
            return name, named_scores[name]
    
    def classify_contour(self, contour):
        desc = ShapeDesc(contour, self.screen_size)
        name, affinity = self.classify_descriptor(desc)
        return name
        
