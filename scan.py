# import the necessary packages
from pyimagesearch.transform import four_point_transform
from pyimagesearch import imutils
from skimage.filters import threshold_local
import numpy as np
import argparse
import cv2
import stl_tools
import os


# # construct the argument parser and parse the arguments
# ap = argparse.ArgumentParser()
# ap.add_argument("-i", "--image", required=True,
#                 help="Path to the image to be scanned")
# args = va rs(ap.parse_args())
#
# # load the image and compute the ratio of the old height
# # to the new height, clone it, and resize it
image = cv2.imread('images/floorplan_test_rooms.jpg')


def preprocess(img, debug=False):

    ratio = img.shape[0] / 500.0
    orig = img.copy()
    img = imutils.resize(img, height = 500)

    # convert the image to grayscale, blur it, and find edges
    # in the image
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    gray = cv2.GaussianBlur(gray, (5, 5), 0)
    edged = cv2.Canny(gray, 30, 180)

    if debug:
        # show the original image and the edge detected image
        # print "STEP 1: Edge Detection"
        cv2.imshow("Image", img)
        cv2.imshow("Edged", edged)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

    # find the contours in the edged image, keeping only the
    # largest ones, and initialize the screen contour
    img, cnts, _ = cv2.findContours(edged.copy(), cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
    cnts = sorted(cnts, key=cv2.contourArea, reverse = True)[:5]

    # loop over the contours
    for c in cnts:
        # approximate the contour
        peri = cv2.arcLength(c, True)
        approx = cv2.approxPolyDP(c, 0.02 * peri, True)

        # if our approximated contour has four points, then we
        # can assume that we have found our screen
        if len(approx) == 4:
            screenCnt = approx
            break

    if debug:
        # show the contour (outline) of the piece of paper
        # print "STEP 2: Find contours of paper"
        cv2.drawContours(img, [screenCnt], -1, (0, 255, 0), 2)
        cv2.imshow("Outline", img)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

    # apply the four point transform to obtain a top-down
    # view of the original image
    warped = four_point_transform(orig, screenCnt.reshape(4, 2) * ratio)
    warped_BGR = warped

    cv2.imwrite('static/template.jpg', warped_BGR)

    # convert the warped image to grayscale, then threshold it
    # to give it that 'black and white' paper effect
    warped = cv2.cvtColor(warped, cv2.COLOR_BGR2GRAY)
    warped = threshold_local(warped, 5, offset=10)
    warped = warped.astype("uint8") * 255
    ret, warped_bin = cv2.threshold(warped,80,255,cv2.THRESH_BINARY)

    # masking
    # filter region of interest
    rows, cols = warped_bin.shape[:2]
    bottom_left  = [cols*0.1, rows*0.95]
    top_left     = [cols*0.1, rows*0.1]
    bottom_right = [cols*0.9, rows*0.95]
    top_right    = [cols*0.9, rows*0.1]

    vertices = np.array([[bottom_left, top_left, top_right, bottom_right]], dtype=np.int32)

    mask = np.zeros_like(warped_bin)
    if len(mask.shape) == 2:
        cv2.fillPoly(mask, vertices, 255)
    else:
        cv2.fillPoly(mask, vertices, (255,)*mask.shape[2])

    warped_bin = cv2.bitwise_and(warped_bin, mask)


    # # ersion and dilation
    # kernel = np.ones((3,3), np.uint8)
    # dilated = cv2.dilate(warped_bin, kernel, iterations=10)
    # eroded = cv2.erode(dilated, kernel, iterations=9)

    # smoothing
    # blurred = warped_bin.copy()
    blurred = cv2.bitwise_not(warped_bin)
    blurred = cv2.pyrUp(blurred)
    # cv2.imshow("bitwise", blurred)

    for i in range(15):
        blurred = cv2.medianBlur(blurred, 7)
    # cv2.imshow("blurred", blurred)

    blurred = cv2.pyrDown(blurred)
    _, blurred = cv2.threshold(blurred, 250, 255, cv2.THRESH_BINARY)


    # kernel = cv2.getStructuringElement(cv2.MORPH_CROSS, (3,3))
    kernel = np.ones((9,9), np.uint8)
    eroded = cv2.erode(blurred, kernel, iterations=3)
    dilated = cv2.dilate(eroded, kernel, iterations=2)

    # # skeleton
    # element = cv2.getStructuringElement(cv2.MORPH_CROSS, (3,3))
    # done = False
    #
    # while not done:
    #     eroded = cv2.erode()


    if debug:
        cv2.imshow("Binary Warped",warped_bin)
        cv2.imshow("Dilated", dilated)
        cv2.imshow("Blurred", blurred)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

    # return 0


    # # morphing image into skeleton
    # ret, warped_bin = cv2.threshold(warped,80,255,cv2.THRESH_BINARY_INV)
    #

    #
    # size = np.size(warped)
    # skel = np.zeros_like(warped)
    #
    # element = cv2.getStructuringElement(cv2.MORPH_CROSS,(3,3))
    #
    # done = False
    #
    # while not done:
    #     eroded = cv2.erode(warped,element)
    #     temp = cv2.dilate(eroded,element)
    #     temp = cv2.subtract(warped,temp)
    #     skel = cv2.bitwise_or(skel,temp)
    #     warped = eroded.copy()
    #
    #     zeros = size - cv2.countNonZero(warped)
    #     if zeros==size:
    #         done = True
    #
    # cv2.imshow("skel",skel)
    # cv2.waitKey(0)


    # ret, warped = cv2.threshold(warped,127,255,cv2.THRESH_BINARY_INV)


    # if debug:
    #     warped = cv2.GaussianBlur(warped, (9, 9), 0)
    #     warped_edges = cv2.Canny(warped, 20, 80)
    #     # show the original and scanned images
    #     # print "STEP 3: Apply perspective transform"
    #     cv2.imshow("Original", imutils.resize(orig, height = 650))
    #     cv2.imshow("Scanned", imutils.resize(warped, height = 650))
    #     cv2.imshow("Warped edges", imutils.resize(warped_edges))
    #     cv2.waitKey(0)

    cv2.imwrite('dilated.jpg', dilated)

    return dilated, warped_bin, warped_BGR


# def find_corners(original, warped):
#     gray = cv2.cvtColor(original,cv2.COLOR_BGR2GRAY)
#
#     gray = np.float32(gray)
#     dst = cv2.cornerHarris(gray,2,3,0.04)
#
#     #result is dilated for marking the corners, not important
#     dst = cv2.dilate(dst,None)
#
#     # Threshold for an optimal value, it may vary depending on the image.
#     original[dst>0.01*dst.max()]=[0,0,255]
#
#     cv2.imshow('dst',original)
#     if cv2.waitKey(0) & 0xff == 27:
#         cv2.destroyAllWindows()

def find_lines(original, edges, min_length=50, max_gap=3, debug=False):

    # filter region of interest
    rows, cols = edges.shape[:2]
    bottom_left  = [cols*0.1, rows*0.95]
    top_left     = [cols*0.1, rows*0.1]
    bottom_right = [cols*0.9, rows*0.95]
    top_right    = [cols*0.9, rows*0.1]

    vertices = np.array([[bottom_left, top_left, top_right, bottom_right]], dtype=np.int32)

    mask = np.zeros_like(edges)
    if len(mask.shape) == 2:
        cv2.fillPoly(mask, vertices, 255)
    else:
        cv2.fillPoly(mask, vertices, (255,)*mask.shape[2])

    edges = cv2.bitwise_and(edges, mask)

    #
    #
    # linesS = cv2.HoughLines(edges,1,np.pi/180, 150)
    # line_imgS = original.copy()
    #
    # # Draw the lines
    # if linesS is not None:
    #     for i in range(0, len(linesS)):
    #         rho = linesS[i][0][0]
    #         theta = linesS[i][0][1]
    #         a = np.cos(theta)
    #         b = np.sin(theta)
    #         x0 = a * rho
    #         y0 = b * rho
    #         pt1 = (int(x0 + 1000*(-b)), int(y0 + 1000*(a)))
    #         pt2 = (int(x0 - 1000*(-b)), int(y0 - 1000*(a)))
    #         cv2.line(line_imgS, pt1, pt2, (0,0,255), 2, cv2.LINE_AA)

    lines = cv2.HoughLinesP(edges,1,np.pi/180,50,min_length,max_gap)
    line_imgP = original.copy()

    for line in lines:
        for x1,y1,x2,y2 in line:
            cv2.line(line_imgP,(x1,y1),(x2,y2),(0,255,0),2)
        # break

    if debug:
        cv2.imshow("Probabilistic Hough", line_imgP)
        # cv2.imshow("Standard Hough", line_imgS)
        cv2.waitKey(0)

    return lines


def detect_lines(original, warped_edges):
    lsd = cv2.createLineSegmentDetector(0)
    lines = lsd.detect(warped_edges)[0]
    drawn = lsd.drawSegments(original, lines)
    cv2.imshow('lol', drawn)
    cv2.waitKey(0)
    for line in lines:
        print(line)


def cluster_lines(lines):
    # TODO: find lines of same slope AND within boundary of one another
    # TODO: cluster them and use their x1y1 x2y2 to generate best fit line
    # TODO: max and min coordinate pair used as cutoff points
    #
    # TODO OR
    #
    # TODO: find lines of same slope AND within boundary of one another
    # TODO max and min coord pair used as keystone points
    # TODO draw lines from point to point

    vertical = []
    horizontal = []
    diag_left = []  # \
    diag_right = [] # /

    line_id = 0

    for line in lines:
        for x1,y1,x2,y2 in line:
            if (x2-x1) == 0:
                slope = 9999999.0
            else:
                slope = (y2 - y1)/(x2 - x1)
            x_centroid = (x2 + x1)/2
            y_centroid = (y2 + y1)/2

            if -0.01 <= slope <= 0.01:
                horizontal.append((line_id, slope, x_centroid, y_centroid))
            elif slope == 9999999.0 or slope > 50:
                vertical.append((line_id, slope, x_centroid, y_centroid))
            elif slope > 0.02:
                diag_left.append((line_id, slope, x_centroid, y_centroid))
            elif slope < -0.02:
                diag_right.append((line_id, slope, x_centroid, y_centroid))
            line_id += 1

    for line in vertical:

        cv2.kmeans()


def get_rooms_area(image, warped_edges, debug=False):  # pass in filtered binary image
    kernel = np.ones((3,3), np.uint8)

    # gray = cv2.cvtColor(original, cv2.COLOR_BGR2GRAY)
    # ret, thresh = cv2.threshold(gray, 128, 255, cv2.THRESH_BINARY_INV)

    thresh = warped_edges

    # thresh = cv2.erode(cv2.dilate(thresh, kernel, iterations=150), kernel, iterations=140)

    im2, contours, hierarchy = cv2.findContours(thresh, cv2.RETR_CCOMP, cv2.CHAIN_APPROX_SIMPLE)

    largest_area = 0

    for cnt in contours:
        if cv2.contourArea(cnt) > largest_area:
            largest_area = cv2.contourArea(cnt)
            largest_contour = cnt

    epsilon = 0.001*cv2.arcLength(largest_contour, True)
    approx = cv2.approxPolyDP(largest_contour, epsilon, True)

    # cv2.drawContours(original, [approx], -1, (0,255,0), 3)

    mask = np.zeros_like(image)

    cv2.drawContours(mask, [approx], -1, 255, -1)

    # (x,y,w,h)=cv2.boundingRect(cnt)
    # cv2.rectangle(mask,(x,y),(x+w,y+h),(255,0,0),2)

    rooms_img = cv2.bitwise_and(mask, image)

    rooms_img = cv2.erode(rooms_img, kernel, iterations=30)

    im3, room_contours, hierarchy = cv2.findContours(rooms_img, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    # test = original.copy()

    # cv2.drawContours(test, room_contours[2], -1, (0,255,0), 3)

    num_rooms = len(room_contours)

    print('Number of rooms:', num_rooms)

    if debug:
        cv2.imshow("original", rooms_img)
        cv2.imshow("contour", mask)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

    return largest_area, num_rooms, mask

def convert_to_obj(dilated, mask):
    alpha_channel = dilated.copy()

    for i in range(len(dilated)):
        for j in range(len(dilated[i])):
            if dilated[i][j] == 255:
                alpha_channel[i][j] = 1.0
            else:
                dilated[i][j] = 0.0

    img_RGBA = cv2.merge((dilated, dilated, dilated, dilated))
    cv2.imwrite('test.png', ~img_RGBA)

    test = cv2.bitwise_and(mask, dilated)

    # dilated = np.add(dilated, test)

    dilated = cv2.resize(~dilated, None, fx=0.6, fy=0.6)
    test = cv2.resize(test, None, fx=0.6, fy=0.6)

    # cv2.imshow('test', test)
    # cv2.waitKey(0)

    stl_tools.numpy2stl(test, 'static/floor.stl', scale=0.03, solid=True, mask_val=5)
    stl_tools.numpy2stl(dilated, 'static/walls.stl', scale=0.8, solid=True, mask_val=5)

    os.system('./meshconv -c obj -tri static/walls.stl')
    os.system('./meshconv -c obj -tri static/floor.stl')

    # stl = pymesh.load_mesh('house.stl')

    # pymesh.save_mesh('house.obj')

# dilated, warped_edges, warped_BGR = preprocess(image, debug=False)
# largest_area, num_rooms, mask = get_rooms_area(dilated, warped_edges)
# convert_to_obj(dilated, mask)
# get_rooms_area(dilated, warped_edges, False)
# find_corners(warped_BGR, warped_edges)
# lines = find_lines(warped_BGR, warped_edges, debug=True)
# detect_lines(warped_BGR, warped_edges)
# cluster_lines(lines)
