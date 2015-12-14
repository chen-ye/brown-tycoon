import sys
sys.path.insert(0, '/usr/local/lib/python2.7/site-packages')

import numpy as np
import cv2
import glob
import os
from threading import Thread
SCALE = 0.5
IMAGE_SIZE = int(1920 * SCALE), int(1080 * SCALE)
CAMERA_NUM = 0

top_left = (112, 69)
top_right = (895, 96)
bottom_left = (250, 421)
bottom_right = (720, 449)
source_size = (960, 540)
points = [top_left, top_right, bottom_left, bottom_right]
points = [(float(x) / source_size[0] * IMAGE_SIZE[0], float(y) / source_size[1] * IMAGE_SIZE[1]) for (x, y) in points]
target_points = [(IMAGE_SIZE[0]*x, IMAGE_SIZE[1]*y) for (x, y) in [(0,0), (1,0), (0,1), (1,1)]]

def points_to_np(points):
    return np.array([np.array(n) for n in points]).astype(np.float32)

# perspective_matrix = np.zeros((3,3), np.float32)
perspective_matrix = cv2.getPerspectiveTransform(points_to_np(points), points_to_np(target_points))

cv2.namedWindow('image')
cv2.setWindowProperty("image", cv2.WND_PROP_FULLSCREEN, True)

IMAGE = None

def photo_loop():
    for fname in os.listdir('chessboard'):
        if fname[0] == '.': continue
        frame = cv2.imread('chessboard/' + fname)
        frame = cv2.resize(frame, IMAGE_SIZE)
        # frame = cv2.undistort(frame, mtx, dist, None, newcameramtx)
        frame = cv2.warpPerspective(frame, perspective_matrix, IMAGE_SIZE)
        cv2.imshow('image', frame)
        cv2.waitKey(0)

def video_loop():
    cap = cv2.VideoCapture(CAMERA_NUM)
    
    while(True):
        # Capture frame-by-frame
        ret, frame = cap.read()
        frame = cv2.resize(frame, IMAGE_SIZE)
        # frame = cv2.undistort(frame, mtx, dist, None, newcameramtx)
        frame = cv2.warpPerspective(frame, perspective_matrix, IMAGE_SIZE)
        cv2.imshow('image', frame)
        global IMAGE
        IMAGE = frame
    
    cap.release()
    cv2.destroyAllWindows()

def start_server():
    import SimpleHTTPServer
    import SocketServer
    import time
    
    class Handler(SimpleHTTPServer.SimpleHTTPRequestHandler):
        def do_GET(self):
            response = "Hello, world"
            content_type = 'text/plain'
            
            if self.path == '/image':
                while IMAGE is None:
                    time.sleep(1)
                response = cv2.imencode('.png', IMAGE)[1].tostring()
                print 'RESP', len(response)
                content_type = 'image/png'
            
            self.send_response(200)
            self.send_header('Content-Type', content_type)
            self.end_headers()
            self.wfile.write(response)
            return
            # return SimpleHTTPServer.SimpleHTTPRequestHandler.do_GET(self)

    Handler = Handler
    server = SocketServer.TCPServer(('0.0.0.0', 8081), Handler)
    
    server.serve_forever()

if __name__ == '__main__':
    t = Thread(target=start_server)
    t.daemon = True
    t.start()
    # mtx, roi, newcameramtx, dist = get_transform()
    if False:
        photo_loop()
    else:
        video_loop()


