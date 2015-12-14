
import os
import urllib2

def shellquote(s):
    return "'" + s.replace("'", "'\\''") + "'"

def start():
    # start the blanker server:
    folder = os.path.split(__file__)[0]
    os.system("open " + shellquote(os.path.join(folder, 'ScreenBlanker.app')))
    

def set_blank(blank):
    path = '/blank' if blank else '/unblank'
    url = 'http://localhost:8090' + path
    req = urllib2.Request(url, "")
    response = urllib2.urlopen(req)
    result = response.read()
    # print result

if __name__ == '__main__':
    start()
    import time
    time.sleep(1)
    set_blank(True)
    time.sleep(1)
    set_blank(0)
