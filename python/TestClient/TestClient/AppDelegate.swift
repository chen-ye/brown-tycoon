//
//  AppDelegate.swift
//  TestClient
//
//  Created by Nate Parrott on 12/11/15.
//  Copyright © 2015 Nate Parrott. All rights reserved.
//

import Cocoa
import WebKit

@NSApplicationMain
class AppDelegate: NSObject, NSApplicationDelegate, NSWindowDelegate {

    @IBOutlet weak var window: NSWindow!
    var webView: WKWebView!
    
    @IBOutlet var layer: CALayer!
    
    @IBOutlet var warpView: AZWarpView!


    func applicationDidFinishLaunching(aNotification: NSNotification) {
        window.contentView!.wantsLayer = true
        layer = CALayer()
        layer.backgroundColor = NSColor.redColor().CGColor
        window.contentView!.layer!.addSublayer(layer)
        layer.bounds = CGRectMake(0, 0, window.frame.size.width, window.frame.size.height)
        //layer.transform = CATransform3DMakeScale(-1, 1, 1)
        
        webView = WKWebView(frame: NSMakeRect(0, 0, 400, 300))
        window.contentView!.addSubview(webView)
        
        refreshImage()
        webView.hidden = true
        webView.loadRequest(NSURLRequest(URL: NSURL(string: "http://localhost:8080/html/calibrator")!))
    }

    func windowDidResize(notification: NSNotification) {
        if let l = layer {
            l.bounds = CGRectMake(0, 0, window.frame.size.width, window.frame.size.height)
        }
    }
    
    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }
    
    func refreshImage() {
        // let url = "https://upload.wikimedia.org/wikipedia/en/thumb/d/d3/Starbucks_Corporation_Logo_2011.svg/1017px-Starbucks_Corporation_Logo_2011.svg.png"
        let url = "http://localhost:8080/image"
        let req = NSURLRequest(URL: NSURL(string: url)!)
        let task = NSURLSession.sharedSession().dataTaskWithRequest(req) { (let dataOpt, _, _) -> Void in
            if let data = dataOpt, let image = NSImage(data: data) {
                print("Got image")
                dispatch_async(dispatch_get_main_queue(), { () -> Void in
                    self.layer.contents = image
                })
                // self.imageView.image = image
            } else {
                print("Failed to download image")
            }
        }
        task.resume()
        
        let oneSec = dispatch_time(DISPATCH_TIME_NOW, Int64(1 * Double(NSEC_PER_SEC)))
        dispatch_after(oneSec, dispatch_get_main_queue()) {
            self.refreshImage()
        }
    }
    
    var prevRotation: CGFloat = 0
    @IBAction func rotate(sender: NSRotationGestureRecognizer) {
        if sender.state == .Began {
            prevRotation = 0
        }
        applyTransform(CATransform3DMakeRotation(sender.rotation - prevRotation, 0, 0, 1))
        prevRotation = sender.rotation
    }
    
    var prevPan: NSPoint = NSZeroPoint
    @IBAction func pan(sender: NSPanGestureRecognizer) {
        if sender.state == .Began {
            prevPan = NSZeroPoint
        }
        let t = sender.translationInView(window.contentView)
        applyTransform(CATransform3DMakeTranslation(t.x - prevPan.x, t.y - prevPan.y, 0))
        prevPan = t
    }
    
    var prevScale: CGFloat = 1
    @IBAction func pinch(sender: NSMagnificationGestureRecognizer) {
        if sender.state == .Began {
            prevScale = 1
        }
        applyTransform(CATransform3DMakeScale((sender.magnification + 1) / prevScale * 0.5 + 0.5, (sender.magnification + 1) / prevScale, 1))
        prevScale = sender.magnification + 1
    }
    
    func applyTransform(t: CATransform3D) {
        layer.transform = CATransform3DConcat(layer.transform, t)
    }
}

