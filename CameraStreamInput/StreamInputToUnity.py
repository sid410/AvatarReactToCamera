import socket
import numpy as np
import cv2
from realsensecv import RealsenseCapture #taken from https://qiita.com/yumion/items/6eeb820c1f06839d57a7


# Create UDP socket at client side 
serverAddressPort   = ("127.0.0.1", 11000)
UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

# For sending status message
lastMsg = ""

# Hand Gesture variables and constants
background = None
hand = None
frames_elapsed = 0
FRAME_HEIGHT = 480
FRAME_WIDTH = 640
CALIBRATION_TIME = 30
BG_WEIGHT = 0.5
OBJ_THRESHOLD = 18

class HandData:
    top = (0,0)
    bottom = (0,0)
    left = (0,0)
    right = (0,0)
    centerX = 0
    prevCenterX = 0
    isInFrame = False
    isWaving = False
    fingers = None
    gestureList = []
    
    def __init__(self, top, bottom, left, right, centerX):
        self.top = top
        self.bottom = bottom
        self.left = left
        self.right = right
        self.centerX = centerX
        self.prevCenterX = 0
        isInFrame = False
        isWaving = False
        
    def update(self, top, bottom, left, right):
        self.top = top
        self.bottom = bottom
        self.left = left
        self.right = right
        
    def check_for_waving(self, centerX):
        self.prevCenterX = self.centerX
        self.centerX = centerX
        
        if abs(self.centerX - self.prevCenterX > 3):
            self.isWaving = True
        else:
            self.isWaving = False


# === Functions for sending string messages to server in Unity ===

def send_string(strVal):
    strEncoded = strVal.encode('ascii')
    hexVal = strEncoded.hex()
    hexLength = int(len(hexVal) / 2)
    hexLengthPlus4 = hexLength + 4
    fullMessage = hexLengthPlus4.to_bytes(4, 'big') + hexLength.to_bytes(4, 'big') + bytes.fromhex(hexVal)
    UDPClientSocket.sendto(fullMessage, serverAddressPort)

def send_status_msg():
    global lastMsg
    msg = "Searching"

    if frames_elapsed < CALIBRATION_TIME:
        msg = "Calibrating"
    elif hand == None or hand.isInFrame == False:
        msg = "NoHand"
    else:
        if hand.isWaving:
            msg = "Waving"
        elif hand.fingers == 0:
            msg = "Rock"
        elif hand.fingers == 1:
            msg = "Pointing"
        elif hand.fingers == 2:
            msg = "Scissors"

    if msg != lastMsg:
        send_string("status:" + msg)
        print("status:" + msg)

    lastMsg = msg


#  === Start of definitions for Hand Gesture Recognition from https://github.com/ishfulthinking/Python-Hand-Gesture-Recognition ===

def highlight_roi(frame):
    # Highlight the region of interest.
    cv2.rectangle(frame, (region_left, region_top), (region_right, region_bottom), (255,255,255), 2)
    cv2.circle(frame, (int(FRAME_WIDTH/2), int(FRAME_HEIGHT/2)), 5, (0, 0, 255), 5)

def get_region(frame):
    # Separate the region of interest from the rest of the frame.
    region = frame[region_top:region_bottom, region_left:region_right]
    # Make it grayscale so we can detect the edges more easily.
    region = cv2.cvtColor(region, cv2.COLOR_BGR2GRAY)
    # Use a Gaussian blur to prevent frame noise from being labeled as an edge.
    region = cv2.GaussianBlur(region, (5,5), 0)

    return region

def get_average(region):
    # We have to use the global keyword because we want to edit the global variable.
    global background
    # If we haven't captured the background yet, make the current region the background.
    if background is None:
        background = region.copy().astype("float")
        return
    # Otherwise, add this captured frame to the average of the backgrounds.
    cv2.accumulateWeighted(region, background, BG_WEIGHT)

# Here we use differencing to separate the background from the object of interest.
def segment(region):
    global hand
    # Find the absolute difference between the background and the current frame.
    diff = cv2.absdiff(background.astype(np.uint8), region)

    # Threshold that region with a strict 0 or 1 ruling so only the foreground remains.
    thresholded_region = cv2.threshold(diff, OBJ_THRESHOLD, 255, cv2.THRESH_BINARY)[1]

    # Get the contours of the region, which will return an outline of the hand.
    contours, hierarchy = cv2.findContours(thresholded_region.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    # If we didn't get anything, there's no hand.
    if len(contours) == 0:
        if hand is not None:
            hand.isInFrame = False
        return
    # Otherwise return a tuple of the filled hand (thresholded_region), along with the outline (segmented_region).
    else:
        if hand is not None:
            hand.isInFrame = True
        segmented_region = max(contours, key = cv2.contourArea)
        return (thresholded_region, segmented_region)

def get_hand_data(thresholded_image, segmented_image):
    global hand
    
    # Enclose the area around the extremities in a convex hull to connect all outcroppings.
    convexHull = cv2.convexHull(segmented_image)
    
    # Find the extremities for the convex hull and store them as points.
    top    = tuple(convexHull[convexHull[:, :, 1].argmin()][0])
    bottom = tuple(convexHull[convexHull[:, :, 1].argmax()][0])
    left   = tuple(convexHull[convexHull[:, :, 0].argmin()][0])
    right  = tuple(convexHull[convexHull[:, :, 0].argmax()][0])
    
    # Get the center of the palm, so we can check for waving and find the fingers.
    centerX = int((left[0] + right[0]) / 2)
    
    # We put all the info into an object for handy extraction (get it? HANDy?)
    if hand == None:
        hand = HandData(top, bottom, left, right, centerX)
    else:
        hand.update(top, bottom, left, right)
    
    # Only check for waving every 6 frames.
    if frames_elapsed % 6 == 0:
        hand.check_for_waving(centerX)
    
    # We count the number of fingers up every frame, but only change hand.fingers if
    # 12 frames have passed, to prevent erratic gesture counts.
    hand.gestureList.append(count_fingers(thresholded_image))
    if frames_elapsed % 12 == 0:
        hand.fingers = most_frequent(hand.gestureList)
        hand.gestureList.clear()

def count_fingers(thresholded_image):
    
    # Find the height at which we will draw the line to count fingers.
    line_height = int(hand.top[1] + (0.2 * (hand.bottom[1] - hand.top[1])))
    
    # Get the linear region of interest along where the fingers would be.
    line = np.zeros(thresholded_image.shape[:2], dtype=int)
    
    # Draw a line across this region of interest, where the fingers should be.
    cv2.line(line, (thresholded_image.shape[1], line_height), (0, line_height), 255, 1)
    
    # Do a bitwise AND to find where the line intersected the hand -- this is where the fingers are.
    line = cv2.bitwise_and(thresholded_image, thresholded_image, mask = line.astype(np.uint8))
    
    # Get the line's new contours. The contours are basically just little lines formed by gaps 
    # in the big line across the fingers, so each would be a finger unless it's very wide.
    contours, hierarchy = cv2.findContours(line.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
    
    fingers = 0
    
    # Count the fingers by making sure the contour lines are "finger-sized", i.e. not too wide.
    # This prevents a "rock" gesture from being mistaken for a finger.
    for curr in contours:
        width = len(curr)
        
        if width < 3 * abs(hand.right[0] - hand.left[0]) / 4 and width > 5:
            fingers += 1
    
    return fingers

def most_frequent(input_list):
    dict = {}
    count = 0
    most_freq = 0
    
    for item in reversed(input_list):
        dict[item] = dict.get(item, 0) + 1
        if dict[item] >= count :
            count, most_freq = dict[item], item
    
    return most_freq

#  === End of definitions for Hand Gesture Recognition ===


# Our region of interest will be the top right part of the frame.
region_top = 0
region_bottom = int(2 * FRAME_HEIGHT / 3)
region_left = int(FRAME_WIDTH / 2)
region_right = FRAME_WIDTH
frames_elapsed = 0


# Realsense capture frame, get depth at center, then send
cap = RealsenseCapture()
cap.WIDTH = FRAME_WIDTH
cap.HEIGHT = FRAME_HEIGHT
cap.FPS = 30
cap.start()

print("started streaming")


while True:
    # Get separated color and depth frames from realsense
    ret, frames = cap.read()
    color_frame = frames[0]
    depth_frame = frames[1]

    # Resize it to the window size.
    frame = cv2.resize(color_frame, (FRAME_WIDTH, FRAME_HEIGHT))

    # Separate the region of interest and prep it for edge detection.
    region = get_region(frame)
    if frames_elapsed < CALIBRATION_TIME:
        get_average(region)
    else:
        region_pair = segment(region)
        if region_pair is not None:
            # If we have the regions segmented successfully, show them in another window for the user.
            (thresholded_region, segmented_region) = region_pair
            cv2.drawContours(region, [segmented_region], -1, (255, 255, 255))
            cv2.imshow("Segmented Image", region)
            
            get_hand_data(thresholded_region, segmented_region)

    # Show the previously captured frame.
    highlight_roi(frame)
    cv2.imshow("Camera Input", frame)
    frames_elapsed += 1

    # Get depth data
    centerDepth = cap.depth_frame.get_distance(int(FRAME_WIDTH/2), int(FRAME_HEIGHT/2))

    # Send depth data and state
    send_string("depth:" + str(centerDepth))
    send_status_msg()

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
print("stopped streaming")