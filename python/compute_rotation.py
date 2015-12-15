import numpy as np
import dumbcv as cv2
import math

def show(img):
    return
    cv2.imshow('rotated', img)
    cv2.waitKey(0)

def contour_to_normalized_points(contour, out_size, scale):
    x1, y1 = contour[0][0]
    minx = x1
    maxx = x1
    miny = y1
    maxy = y1
    for pt in contour:
        x, y = pt[0]
        minx = min(minx, x)
        maxx = max(maxx, x)
        miny = min(miny, y)
        maxy = max(maxy, y)
    
    w = (maxx - minx)
    h = (maxy - miny)
    translate_x = 0 - (minx/2 + maxx/2) # put the center at the origin
    translate_y = 0 - (miny/2 + maxy/2)
    scale = min(out_size * scale / w, out_size * scale / h)
    """print 'scale', scale
    print 'translate', translate_x
    print 'minx', minx
    print 'maxx', maxx"""
    
    out = []
    for pt in contour:
        x, y = pt[0]
        x = round((x + translate_x) * scale + out_size/2)
        y = round((y + translate_y) * scale + out_size/2)
        out.append(np.array([x,y]))
    return np.array(out, dtype=np.int32)

def compute_rotation(contour1, contour2):
    resolution = 50
    scale = 0.5
    val = 1 # set to 1 for use, 255 for debugging
    
    shape1 = contour_to_normalized_points(contour1, resolution, scale)
    shape2 = contour_to_normalized_points(contour2, resolution, scale)
    # print shape1
    num_angles = 15
    img1 = np.zeros((resolution, resolution), np.uint8)
    cv2.fillPoly(img1, [shape1], val)
    img1_flipped = cv2.flip(img1, 1)
    show(img1)
        
    similarities = []
    transforms = []
    for rotation in [x * math.pi * 2.0 / num_angles for x in xrange(num_angles)]:
        for flip in (False, True):
            transforms.append((rotation, flip))
    img2 = np.zeros((resolution, resolution), np.uint8)
    cv2.fillPoly(img2, [shape2], val)
    
    for rotation, flip in transforms:
        M = cv2.getRotationMatrix2D((resolution/2, resolution/2), rotation * 180.0 / math.pi, 1.0)
        img2_rotated = cv2.warpAffine(img2, M, (resolution, resolution))
        show(img2_rotated)
        overlap = np.multiply((img1_flipped if flip else img1), img2_rotated)
        x = np.sum(overlap) * 1.0 / (resolution ** 2)
        similarities.append(x)
    
    best_idx = max(range(len(similarities)), key=lambda i: similarities[i])
    best_non_flipped_idx = max(range(0, len(similarities), 2), key=lambda i: similarities[i])
    rot, flipped = transforms[best_idx]
    rot_without_flipping = transforms[best_non_flipped_idx]
    return rot, flipped, rot_without_flipping
