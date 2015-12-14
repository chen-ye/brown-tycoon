
SAMPLES = 7
import math
import itertools
import numpy as np

def pt_dist(pt1, pt2):
    return ((pt1[0] - pt2[0]) ** 2 + (pt1[1] - pt2[1]) ** 2) ** 0.5

def enumerate_segments(contour):
    return itertools.chain.from_iterable(itertools.repeat(zip(contour, contour[1:] + [contour[0]])))

def contour_to_token(contour):
    
    total_dist = 0
    for p1, p2 in zip(contour, contour[1:] + [contour[0]]):
        dist = pt_dist(p1[0], p2[0])
        total_dist += dist
    
    dist_between_samples = total_dist / SAMPLES
    dist_to_next_sample = 0
    samples = []
    
    for p1, p2 in enumerate_segments(contour):
        pt1 = p1[0]
        pt2 = p2[0]
        dist = pt_dist(pt1, pt2)
        
        angle = math.atan2(pt2[1] - pt1[1], pt2[0] - pt1[0])
        progress = 0
        while True:
            remaining_dist = dist * (1 - progress)
            if dist_to_next_sample > remaining_dist:
                dist_to_next_sample -= remaining_dist
                break
            else:
                progress += dist_to_next_sample / dist
                samples.append(angle)
                dist_to_next_sample = dist_between_samples
                if len(samples) == SAMPLES:
                    break
        
        if len(samples) == SAMPLES:
            break
    assert len(samples) == SAMPLES
    return samples

def angle_diff(a1, a2):
    pi2 = math.pi * 2
    a1 = a1 % pi2
    a2 = a2 % pi2
    d = abs(a1 - a2)
    return min(d, pi2 - d)

def dev(nums, p=2):
    l = len(nums)
    mean = sum(nums) / l
    return (sum([abs((x - mean) ** p) for x in nums]) / l) ** (1.0 / p)

def _token_diff(t1, t2, offset=0):
    t2 = t2[offset:] + t2[:offset]
    diffs = [angle_diff(a1, a2) for a1, a2 in zip(t1, t2)]
    # print dev(diffs, p=2)
    # print diffs
    return dev(diffs, p=1)

def negate_token(t):
    return list([x + math.pi for x in reversed(t)])

def token_diff(t1, t2):
    diffs = [_token_diff(t1, t2, offset) for offset in xrange(len(t1))]
    t2_neg = negate_token(t2)
    # print
    # print t1
    # print t2_neg
    diffs += [_token_diff(t1, t2_neg, offset) for offset in xrange(len(t1))]
    return min(diffs)

if __name__ == '__main__':
    triangleA1 = [[[0,0]], [[1,0]], [[0,1]]]
    triangleA2 = [[[1,0]], [[0,0]], [[0,1]]]
    # print contour_to_token(triangleA1)
    # print contour_to_token(squareA1)
    
    squareA1 = [[[0,0]], [[0,1]], [[1,1]], [[1,0]]]
    squareA2 = list(reversed(squareA1))
    
    print 'same triangle, point-swapped', token_diff(contour_to_token(triangleA1), contour_to_token(triangleA2))
    print 'same square', token_diff(contour_to_token(squareA1), contour_to_token(squareA1))
    print 'reversed square', token_diff(contour_to_token(squareA1), contour_to_token(squareA2))
    print 'square and triangle', token_diff(contour_to_token(squareA1), contour_to_token(triangleA1))

