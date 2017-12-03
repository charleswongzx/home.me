from flask import Flask
from flask import request
from flask import send_file
from flask import jsonify
import subprocess
import requests

from PIL import Image
import numpy as np
import cv2

from scan import preprocess, get_rooms_area, convert_to_obj

app = Flask(__name__)

area = 0
num_rooms = 0
token = ''

@app.route('/')
def hello_world():
    return 'hello world'

@app.route('/process_img', methods=['POST'])
def process():
    global area, num_rooms
    # content = request.get_json(silent=True)
    img = Image.open(request.files['file'])
    print('Image received')

    img = np.array(img)
    # Convert RGB to BGR
    img = img[:, :, ::-1]
    # print('Converting image...')

    filename = 'static/warped_img.jpg'

    dilated, warped_edges, warped_BGR = preprocess(img, debug=False)
    cv2.imwrite(filename, dilated)

    area, num_rooms, mask = get_rooms_area(dilated, warped_edges, debug=True)

    convert_to_obj(dilated, mask)

    # return area, num rooms, processed image png

    # return render_template('floorplan.html', area=area, num_rooms=num_rooms, img=send_file(filename, mimetype='image/jpeg'))

    # return send_file(filename, mimetype='image/jpeg')

    room_dict = {'area': area, 'num_rooms': num_rooms}
    return jsonify(room_dict)


# @app.route('/get_rooms', methods=['GET'])
# def rooms():
#     room_dict = {'area': area, 'num_rooms': num_rooms}
#     return jsonify(room_dict)


@app.route('/get_walls', methods=['GET'])
def walls():
    filename = 'static/walls.obj'
    return send_file(filename, as_attachment=True, attachment_filename='walls.obj')


@app.route('/get_floor', methods=['GET'])
def floor():
    filename = 'static/floor.obj'
    return send_file(filename, as_attachment=True, attachment_filename='floor.obj')


@app.route('/get_token', methods=['GET'])
def get_token():
    global token
    token_dict = {"access_token": token}
    return jsonify(token_dict)


@app.route('/post_token', methods=['GET'])
def post_token():
    global token

    token = request.args['code']

    print(token)

    return 'Success! Return to app.'


