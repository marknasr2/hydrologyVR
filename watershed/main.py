#!flask/bin/python
import os
from flask import Flask, request, jsonify
from flask import request
import pandas as pd
import pickle
from flask_cors import CORS, cross_origin

app = Flask(__name__)
cors = CORS(app, resources={r"/*": {"origins": "*"}})

def _reorder(features: dict) -> list:
    '''Reorder to same order as dfx in model.
    '''
    p = 1
    q = 31
    l = []
    while p <= q:
        l.append(
            float(features['FOREST_COVER' + str(p)])
        )
        l.append(
            float(features['PCP' + str(p)])
        )
        p += 1
    return l

OUTPUTS = [
    'FLOW_OUTcms', 'EVAPcms', 'TLOSScms', 'SED_OUTtons', 'SEDCONCmgL',
    'ORGN_OUTkg', 'ORGP_OUTkg', 'NO3_OUTkg', 'NH4_OUTkg', 'NO2_OUTkg',
    'MINP_OUTkg', 'CHLA_OUTkg', 'CBOD_OUTkg', 'DISOX_OUTkg', 'TOTNkg'
]

def _predict(features: list) -> dict:
    '''Predict using all models.
    '''
    d = {}
    for model in OUTPUTS:
        loaded_model = pickle.load(open('models/{0}.pkl'.format(model), 'rb'))
        # .predict() works on a 2D array, but we are only predicting on a single row.
        prediction = loaded_model.predict([features])
        d[model] = prediction[0]
    return d

@app.route('/alive')
def index():
    return "true"

@app.route('/prediction', methods=['POST'])
def get_prediction():

    # ImmutableMultiDict
    imd = request.form
    # Convert to flat dictionary.
    d = imd.to_dict(flat=True)
    # Reorder for prediction.
    features = _reorder(d)

    # Do predictions.
    predictions = _predict(features)

    return jsonify(predictions)

if __name__ == '__main__':
    app.run(port=5000, host='0.0.0.0')
