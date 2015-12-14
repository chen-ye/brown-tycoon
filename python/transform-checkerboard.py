import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')

import numpy as np
import cv2
import glob
import os
SCALE = 0.5
IMAGE_SIZE = int(1920 * SCALE), int(1080 * SCALE)
CAMERA_NUM = 0

def get_transform():
    # termination criteria
    criteria = (cv2.TERM_CRITERIA_EPS + cv2.TERM_CRITERIA_MAX_ITER, 30, 0.001)
    pattern_size = (9, 6)
    pattern_points = np.zeros( (np.prod(pattern_size), 3), np.float32 )
    pattern_points[:,:2] = np.indices(pattern_size).T.reshape(-1, 2)
    # pattern_points *= square_size
    obj_points = []
    img_points = []
    w,h = 0,0

    for fname in os.listdir('chessboard'):
        if fname[0] == '.': continue
        img = cv2.imread('chessboard/' + fname)
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        h, w = img.shape[:2]
        found, corners = cv2.findChessboardCorners(gray, pattern_size)
        print found
        if found:
            corners2 = cv2.cornerSubPix(gray,corners,(11,11),(-1,-1),criteria)
        
            img_points.append(corners.reshape(-1, 2))
            obj_points.append(pattern_points)
        
            vis = cv2.cvtColor(gray, cv2.COLOR_GRAY2BGR)
            cv2.drawChessboardCorners(vis, pattern_size, corners, found)
        
            # cv2.imshow('img', vis)
            # cv2.waitKey(500)

    ret, mtx, dist, rvecs, tvecs = cv2.calibrateCamera(obj_points, img_points, (w,h), None, None)
    newcameramtx, roi = cv2.getOptimalNewCameraMatrix(mtx,dist,(w,h),1,(w,h))
    return mtx, roi, newcameramtx, dist
    # cv2.destroyAllWindows()

def photo_loop(mtx, roi, newcameramtx, dist):
    for fname in os.listdir('chessboard'):
        if fname[0] == '.': continue
        frame = cv2.imread('chessboard/' + fname)
        frame = cv2.resize(frame, IMAGE_SIZE)
        frame = cv2.undistort(frame, mtx, dist, None, newcameramtx)
        cv2.imshow('image', frame)
        cv2.waitKey(0)
        

def video_loop(mtx, roi, newcameramtx, dist):
    cap = cv2.VideoCapture(CAMERA_NUM)
    
    while(True):
        # Capture frame-by-frame
        ret, frame = cap.read()
        frame = cv2.resize(frame, IMAGE_SIZE)
        frame = cv2.undistort(frame, mtx, dist, None, newcameramtx)
        cv2.imshow('image', frame)
    
    cap.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':
    mtx, roi, newcameramtx, dist = get_transform()
    fn = video_loop if False else photo_loop
    fn(mtx, roi, newcameramtx, dist)


