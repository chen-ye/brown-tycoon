import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')

import cv2
import numpy as np
import os
import pickle
import sys
import uuid
CAMERA = 0
SCALE = 0.5
IMAGE_SIZE = int(1920 * SCALE), int(1080 * SCALE)

def save(img):
    name = uuid.uuid4().hex
    path = 'chessboard/{0}.png'.format(name)
    cv2.imwrite(path, img)

def video_loop():
    cap = cv2.VideoCapture(CAMERA)
    cv2.waitKey(1500)
    
    while(True):
        ret, frame = cap.read()
        frame = cv2.resize(frame, IMAGE_SIZE)
        save(frame)
        cv2.imshow('frame', frame)
        cv2.waitKey(1500)
    
    cap.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':
    video_loop()

