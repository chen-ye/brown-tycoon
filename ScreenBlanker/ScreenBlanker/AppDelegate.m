//
//  AppDelegate.m
//  ScreenBlanker
//
//  Created by Nate Parrott on 12/14/15.
//  Copyright © 2015 Nate Parrott. All rights reserved.
//

#import "AppDelegate.h"
#import <HTTPServer.h>
#import <HTTPConnection.h>
#import <HTTPDataResponse.h>
#import <HTTPFileResponse.h>
#import "SBWindow.h"

@interface SBConnection : HTTPConnection

@end

@implementation SBConnection

- (NSObject<HTTPResponse>*)httpResponseForMethod:(NSString *)method URI:(NSString *)path {
    if ([method isEqualToString:@"POST"] && ([path isEqualToString:@"/blank"] || [path isEqualToString:@"/unblank"])) {
        BOOL blank = [path isEqualToString:@"/blank"];
        NSString *response = blank ? @"blanked" : @"unblanked";
        [(AppDelegate *)[NSApp delegate] setBlanked:blank];
        return [[HTTPDataResponse alloc] initWithData:[response dataUsingEncoding:NSUTF8StringEncoding]];
    } else if ([path isEqualToString:@"/debug"]) {
        return [[HTTPFileResponse alloc] initWithFilePath:[[NSBundle mainBundle] pathForResource:@"debug" ofType:@"html"] forConnection:self];
    } else {
        return [super httpResponseForMethod:method URI:path];
    }
}

- (BOOL)supportsMethod:(NSString *)method atPath:(NSString *)path {
    if ([method isEqualToString:@"POST"]) {
        return  [@[@"/blank", @"/unblank"] containsObject:path];
    } else {
        return [super supportsMethod:method atPath:path];
    }
}

@end

@interface AppDelegate ()

@property (nonatomic) HTTPServer *server;

@property SBWindow *window;
@end

@implementation AppDelegate

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification {
    // Insert code here to initialize your application
    // NSApp.activationPolicy = NSApplicationActivationPolicyAccessory;
    
    self.window = [[SBWindow alloc] initWithContentRect:[NSScreen mainScreen].frame styleMask:NSBorderlessWindowMask backing:NSBackingStoreBuffered defer:NO];
    [self.window setLevel:NSMainMenuWindowLevel + 2];
    self.window.excludedFromWindowsMenu = YES;
    self.window.opaque = YES;
    self.window.ignoresMouseEvents = YES;
    self.window.backgroundColor = [NSColor colorWithWhite:0 alpha:1];
    self.window.collectionBehavior = NSWindowCollectionBehaviorCanJoinAllSpaces | NSWindowCollectionBehaviorStationary | NSWindowCollectionBehaviorIgnoresCycle;
    
    self.server = [HTTPServer new];
    self.server.connectionClass = [SBConnection class];
    self.server.port = 8090;
    NSError *error;
    if (![self.server start:&error]) {
        NSLog(@"Couldn't start screen blanker: %@", error);
    }
}

- (void)setBlanked:(BOOL)blanked {
    dispatch_async(dispatch_get_main_queue(), ^{
        [self.window setIsVisible:blanked];
        if (blanked) {
            [self.window becomeMainWindow];
            // [NSApp activateIgnoringOtherApps:YES];
        }
    });
}

- (void)applicationWillTerminate:(NSNotification *)aNotification {
    // Insert code here to tear down your application
}

@end
