import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')
import cv2

def contour_to_points(contour):
    return np.array([np.array(pt[0]) for pt in contour])

def match_affine(points1, point2):
    mat = cv2.estimateRigidTransform(points1, points2, fullAffine=False)
    proj = points1 
