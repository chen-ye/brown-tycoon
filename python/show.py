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
IMAGE_SIZE = (int(640 * 1.5), int(480 * 1.5))
MAX_CONTOURS_PER_STAGE = 10
MAX_CONTOURS_TO_STORE = 20
MAX_CONTOUR_DISTANCE_BETWEEN_FRAMES = 0.2
USE_EXTERNAL_CAM = True

import zbar
scanner = zbar.Scanner()
# scanner.parse_config('enable') # set_config(zbar.Symbol.QRCODE, zbar.Config.ENABLE, 1)

def process_image(frame):
    frame = cv2.resize(frame, IMAGE_SIZE)
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
    cap = cv2.VideoCapture(1 if USE_EXTERNAL_CAM else 0)
    
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

if __name__ == '__main__':
    video_loop()

