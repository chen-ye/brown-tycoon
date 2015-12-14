//
//  SBWindow.m
//  ScreenBlanker
//
//  Created by Nate Parrott on 12/14/15.
//  Copyright © 2015 Nate Parrott. All rights reserved.
//

#import "SBWindow.h"

@implementation SBWindow

- (NSRect)constrainFrameRect:(NSRect)frameRect toScreen:(NSScreen *)screen {
    return frameRect;
}

- (BOOL)canBecomeKeyWindow {
    return NO;
}

@end
