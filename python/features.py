import cv2
import math

def extra_features(contour):
    points = [pts[0] for pts in contour]
    return [sharpness(points), density(contour)]

def angle_diff(a1, a2):
    pi2 = math.pi * 2
    a1 = a1 % pi2
    a2 = a2 % pi2
    d = abs(a1 - a2)
    return min(d, pi2 - d)

def pt_dist(pt1, pt2):
    return ((pt1[0] - pt2[0]) ** 2 + (pt1[1] - pt2[1]) ** 2) ** 0.5

def direction_changes(points):
    last_angle = 0
    d = 0
    for p1, p2 in zip(points, points[1:] + points[:-1]):
        angle = math.atan2(p2[1] - p1[1], p2[0] - p1[0])
        if angle_diff(last_angle, angle) > math.pi * 0.7:
            last_angle = angle
            d += 1
    return d

def sharpness(points):
    last_angle = None
    diffs = []
    for p1, p2 in zip(points, points[1:] + points[:-1]):
        angle = math.atan2(p2[1] - p1[1], p2[0] - p1[0])
        if last_angle:
            diffs.append(angle_diff(angle, last_angle))
        last_angle = angle
    return sum(diffs) * 1.0 / len(diffs)

def density(contour):
    if len(contour) < 5:
        return 0.5
    ((x,y),(width,height), angle) = cv2.fitEllipse(contour)
    return cv2.contourArea(contour) * 1.0 / (width * height)
