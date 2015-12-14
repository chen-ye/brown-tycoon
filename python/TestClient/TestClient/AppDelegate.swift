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
class AppDelegate: NSObject, NSApplicationDelegate {

    @IBOutlet weak var window: NSWindow!
    @IBOutlet weak var webView: WKWebView!


    func applicationDidFinishLaunching(aNotification: NSNotification) {
        refreshImage()
        webView.loadRequest(NSURLRequest(URL: NSURL(string: "http://localhost:8080/html/calibrator")!))
    }

    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }
    
    func refreshImage() {
        let url = "http://localhost:8080/image"
        let req = NSURLRequest(URL: NSURL(string: url)!)
        let task = NSURLSession.sharedSession().dataTaskWithRequest(req) { (let dataOpt, _, _) -> Void in
            if let data = dataOpt, let image = NSImage(data: data) {
                print("Got image")
                self.imageView.image = image
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

    @IBOutlet var imageView: NSImageView!
}

