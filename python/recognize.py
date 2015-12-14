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
import pickle
import urlparse
import json

import matching

CONTOUR_SIMPLIFICATION = 100 # lower is stronger
BLUR_RADIUS = 3
IMAGE_SIZE_FOR_TRACING = (640, 480)
TRACED_ASPECT_RATIO = IMAGE_SIZE_FOR_TRACING[0] * 1.0 / IMAGE_SIZE_FOR_TRACING[1]
MIN_SHAPE_SIZE = 100
THRESH_RADIUS = 31
THRESH_CONSTANT = 7
MAX_CONTOURS = 5

PORT = 8080

perspective_matrix = None

def points_to_np(points):
    return np.array([np.array(n) for n in points]).astype(np.float32)

def update_calibration(points):
    source_size = (960, 540)
    points = [(float(x) / source_size[0] * IMAGE_SIZE[0], float(y) / source_size[1] * IMAGE_SIZE[1]) for (x, y) in points]
    target_points = [(IMAGE_SIZE[0]*x, IMAGE_SIZE[1]*y) for (x, y) in [(0,0), (1,0), (0,1), (1,1)]]

    global perspective_matrix
    perspective_matrix = cv2.getPerspectiveTransform(points_to_np(points), points_to_np(target_points))

top_left = (112, 69)
top_right = (895, 96)
bottom_left = (250, 421)
bottom_right = (720, 449)
calibration_points = [top_left, top_right, bottom_left, bottom_right]
calibration_points_names = ["top_left", "top_right", "bottom_left", "bottom_right"]
update_calibration(calibration_points)

SHOW_SINGLE_CAPTURE_AND_DUMP = False

if not SHOW_SINGLE_CAPTURE_AND_DUMP:
    names = {
        1: "ratty",
        2: "E",
        0: "fuck",
        3: "shang"
    }
    existing_contours = pickle.loads(open('shapes.pickle').read())
    existing_shapes = [matching.ShapeDesc(contour, IMAGE_SIZE_FOR_TRACING) for contour in existing_contours]
    named_shapes = {name: existing_shapes[i] for i, name in names.iteritems()}
    matcher = matching.Matcher(named_shapes, IMAGE_SIZE_FOR_TRACING)

cv2.namedWindow('image')
cv2.setWindowProperty("image", cv2.WND_PROP_FULLSCREEN, True)

IMAGE = None
OUTPUT = {}

def process_image(frame):
    output = {
        "buildings": []
    }
    shapes = []
    
    frame = cv2.resize(frame, IMAGE_SIZE_FOR_TRACING)
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    blurred = cv2.blur(gray, (BLUR_RADIUS, BLUR_RADIUS))
    edges = cv2.adaptiveThreshold(blurred, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, THRESH_RADIUS, THRESH_CONSTANT)
    retr_external = True
    
    contours, hierarchy = cv2.findContours(edges, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    hierarchy = hierarchy[0]
    top_level_contour_indices = [i for i in range(len(hierarchy)) if -1 == hierarchy[i][3]]
    biggest_top_level_contour_index = max(top_level_contour_indices, key=lambda i: cv2.contourArea(contours[i])) if len(top_level_contour_indices) else None
    shape_contour_indices = [i for i in range(len(hierarchy)) if hierarchy[i][3] == biggest_top_level_contour_index]
    contours = [contours[i] for i in shape_contour_indices]
    
    # print hierarchy
    
    # (contour, id, name, descriptor)
    
    contours = [c for c in contours if cv2.contourArea(c) > MIN_SHAPE_SIZE]
    contours = [cv2.approxPolyDP(c, cv2.arcLength(c, True) / CONTOUR_SIMPLIFICATION, True) for c in contours]
    contours.sort(key=lambda x: cv2.contourArea(x), reverse=True)
    contours = contours[:min(len(contours), MAX_CONTOURS)]
    
    if SHOW_SINGLE_CAPTURE_AND_DUMP:
        for contour in contours:
            cv2.drawContours(frame, [contour], -1, (255,0,0))
            x,y = contour[0][0]
            i = len(shapes)
            cv2.putText(frame, str(i), (x,y), cv2.FONT_HERSHEY_PLAIN, 0.7, (0, 0, 255))
            shapes.append(contour)
    else:
        for (contour, id, name, descriptor) in matcher.run_frame(contours):
            cv2.drawContours(frame, [contour], -1, (255,0,0))
            x,y = contour[0][0]
            cv2.putText(frame, "{0}:{1}".format(id, name), (x,y), cv2.FONT_HERSHEY_PLAIN, 0.7, (0, 0, 255))
            output['buildings'].append(shape_json(contour, id, name))

    if SHOW_SINGLE_CAPTURE_AND_DUMP:
        f = open('last_rec.pickle', 'w')
        f.write(pickle.dumps(shapes))
        f.close()
    return frame, output

def shape_json(contour, id, shape_name):
    points = [ps[0] for ps in contour]
    ((center_x,center_y),(width,height),rotation) = cv2.minAreaRect(points_to_np(points))
    image_width, image_height = IMAGE_SIZE_FOR_TRACING
    return {
        "key": str(id),
        "type": shape_name,
        "x": center_x * 1.0 / image_width * TRACED_ASPECT_RATIO,
        "y": center_y * 1.0 / image_height,
        "width": width * 1.0 / image_width * TRACED_ASPECT_RATIO,
        "height": height * 1.0 / image_height,
        "points": [pts[0] for pts in contour], # [[1,1], [1,3], etc]
        "rotation": rotation
    }

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
        
        frame, output = process_image(frame)
        
        cv2.imshow('image', frame)
        global IMAGE
        IMAGE = frame
        global OUTPUT
        OUTPUT = output
        if SHOW_SINGLE_CAPTURE_AND_DUMP:
            cv2.waitKey(0)
            break
    
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
            
            if self.path.startswith('/html/'):
                name = self.path.split('/html/', 1)[1]
                content_type = 'text/html'
                response = open(name + '.html').read()
            elif self.path.startswith('/image'):
                while IMAGE is None:
                    time.sleep(1)
                response = cv2.imencode('.png', IMAGE)[1].tostring()
                # print 'RESP', len(response)
                content_type = 'image/png'
            elif self.path.startswith('/do_calibrate?'):
                parsed = urlparse.urlparse('http://localhost' + self.path)
                query = urlparse.parse_qs(parsed.query)
                if 'point' in query and 'dx' in query and 'dy' in query:
                    dx = int(query['dx'][0])
                    dy = int(query['dy'][0])
                    point = query['point'][0]
                    if point in calibration_points_names:
                        i = calibration_points_names.index(point)
                        x,y = calibration_points[i]
                        x += dx
                        y += dy
                        calibration_points[i] = (x,y)
                        update_calibration(calibration_points)
                        print "CALIBRATION:", str(calibration_points)
                        response = 'okay'
            elif self.path == '/shapes':
                content_type = 'application/json'
                response = json.dumps(OUTPUT)
                    
            self.send_response(200)
            self.send_header('Content-Type', content_type)
            self.end_headers()
            self.wfile.write(response)
            return
            # return SimpleHTTPServer.SimpleHTTPRequestHandler.do_GET(self)

    Handler = Handler
    print 'Using port', PORT
    SocketServer.TCPServer.allow_reuse_address = True
    server = SocketServer.TCPServer(('0.0.0.0', PORT), Handler)
    
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


