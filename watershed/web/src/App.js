import React, { Component } from 'react';
import {
  Layout,
  Card,
  Page,
  AppProvider,
  FormLayout,
  TextField,
  Button,
} from '@shopify/polaris';
import axios from 'axios';
import Results from './Results';

const api = 'http://localhost:8001'

const defaults = {
  'PCP19': 968.87098826979434, 'PCP18': 968.87087096774121, 'FOREST_COVER8': 30.979363636363608, 'FOREST_COVER9': 13.894272727272725, 'FOREST_COVER4': 25.282454545454488, 'FOREST_COVER5': 46.156454545454629, 'PCP11': 1012.6645219941356, 'FOREST_COVER7': 20.889454545454551, 'PCP17': 968.87129618768347, 'FOREST_COVER1': 21.885636363636419, 'FOREST_COVER2': 25.62436363636364, 'FOREST_COVER3': 39.635181818181813, 'PCP31': 966.92223460410582, 'PCP30': 966.92276539589443, 'PCP13': 1012.6645161290332, 'FOREST_COVER18': 7.2060909090909009, 'PCP12': 968.87074486803454, 'FOREST_COVER16': 13.745181818181807, 'FOREST_COVER17': 21.688000000000105, 'FOREST_COVER14': 10.790181818181839, 'FOREST_COVER15': 12.90781818181812, 'FOREST_COVER12': 25.736999999999956, 'FOREST_COVER6': 70.961545454545742, 'FOREST_COVER10': 49.740181818181782, 'FOREST_COVER11': 27.190272727272706, 'PCP10': 968.87102932551318, 'PCP21': 968.87081524926612, 'PCP28': 966.92196774193599, 'PCP29': 966.92293841642288, 'PCP26': 966.92227859237562, 'PCP27': 966.92299706744961, 'PCP24': 968.87160117302165, 'FOREST_COVER19': 12.413727272727275, 'PCP22': 1012.6642785923761, 'PCP23': 968.87065689149472, 'PCP20': 1012.664906158359, 'PCP16': 968.87065102639247, 'FOREST_COVER30': 48.358909090908917, 'PCP15': 968.87129912023465, 'FOREST_COVER31': 45.663909090909151, 'PCP14': 1012.6642287390029, 'FOREST_COVER29': 34.556181818181777, 'PCP9': 1012.6642609970676, 'PCP8': 1012.664524926687, 'PCP7': 1012.6645102639308, 'PCP6': 1042.3405923753669, 'PCP5': 1012.664724340177, 'PCP4': 1012.6642991202351, 'PCP3': 1012.664307917889, 'PCP2': 1012.6649648093858, 'PCP1': 1012.6644868035196, 'FOREST_COVER13': 54.1667272727274, 'FOREST_COVER27': 36.154727272727321, 'FOREST_COVER26': 9.7670000000000332, 'FOREST_COVER25': 14.56472727272728, 'FOREST_COVER24': 23.29572727272727, 'FOREST_COVER23': 9.2115454545454476, 'FOREST_COVER22': 28.020909090909122, 'FOREST_COVER21': 33.041999999999959, 'FOREST_COVER20': 3.6105454545454609, 'PCP25': 1006.3815014662775, 'FOREST_COVER28': 19.144000000000037
}

class App extends Component {

  state = {
    features: defaults,
    displayResult: false,
    result: '',
    previousResult: {},
    createResult: {},
    totalpcp: 0.0,
    previouspcp: 0.0,
  }

  // Tabulate feature values on start.
  componentWillMount(){
    // The model treats precipiation separately per subbasin.
    // In order to provide a single sum, we change the percantage of all values.
    let totalpcp = 0.0
    let features = this.state.features
    for (let key in features) {
      if (key.startsWith('PCP')) {
        totalpcp = features[key] + totalpcp
      }
    }
    this.setState({totalpcp})
    this.setState({previouspcp: totalpcp})
    this.createDefault()
  }

  // Modifies feature values by some float.
  // Use only on final submit.
  mod = ( prefix, value ) => {
    let features = this.state.features
    for (let key in features){
      if (key.startsWith(prefix)){
        let n = features[key] * value
        features[key] = n
      }
    }
    this.setState({features: features})
  }

  setTotal = (key, value) => {
    this.setState({ [key]: value})
  }

  // Sets the mod values in response to form.
  set = ( key, value ) => {
    let newFeatures = this.state.features
    newFeatures[key] = value
    this.setState({ features: newFeatures })
  }

  // Supports the back button in Results
  handleBack = () => {
    this.setState({displayResult: false})
  }

  createDefault = () => {
    // Create FormData.
    let data = new FormData()
    for (let k in this.state.features) {
      data.append(k, this.state.features[k])
    }

    axios.post(api + '/prediction', data)
      .then(response => {
        console.log(response)
        let r = response.data
        this.setState({ currentResult: r})
      })
  }

  // Submits the model parameters to the api.
  submit = () => {
    this.setState({ previousResult: this.state.currentResult })
    // Sync totalpcp with subasin-specific pcp.
    this.setState({ previouspcp: this.state.totalpcp })
    let modpcp = this.state.totalpcp / this.state.previouspcp
    this.mod('PCP', modpcp)
    
    // Create FormData.
    let data = new FormData()
    for(let k in this.state.features){
      data.append(k, this.state.features[k])
    }

    axios.post(api+'/prediction', data)
    .then(response => {
      // console.log(response)
      let r = response.data
      let rows = []
      for (let k in r) {
        console.log(this.state.previousResult)
        rows.push([k, this.state.previousResult[k], r[k]])
      }
      console.log(rows)
      let result = Results(rows, this.handleBack)
      console.log(result)
      this.setState({ currentResult: r })
      this.setState({ result: result })
      this.setState({ displayResult: true })
    })
  }

  render() 
  {
    const {
      displayResult,
      result,
      totalpcp,
    } = this.state;

    const {
      set,
      setTotal,
      submit,
    } = this;

    return (
      <AppProvider>
        <Page title="South Nation Watershed">
          {!displayResult?
            <Layout>
              <Layout.Section>
                <Card title="Stream Network" sectioned>
                  <img style={{ maxHeight: 400 }} src={require('./images/snbasins.png')} alt='basin flows' />
                </Card>
              </Layout.Section>
              <Layout.Section secondary>
                <Card title="Basin Parameters" sectioned>
                  <FormLayout>
                    <TextField
                      type='number'
                      label="Total Precipitation in Watershed"
                      value={totalpcp}
                      onChange={(e) => setTotal('totalpcp', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 1"
                      value={this.state.features.FOREST_COVER1}
                      onChange={(e) => set('FOREST_COVER1', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 2"
                      value={this.state.features.FOREST_COVER2}
                      onChange={(e) => set('FOREST_COVER2', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 3"
                      value={this.state.features.FOREST_COVER3}
                      onChange={(e) => set('FOREST_COVER3', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 4"
                      value={this.state.features.FOREST_COVER4}
                      onChange={(e) => set('FOREST_COVER4', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 5"
                      value={this.state.features.FOREST_COVER5}
                      onChange={(e) => set('FOREST_COVER5', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 6"
                      value={this.state.features.FOREST_COVER6}
                      onChange={(e) => set('FOREST_COVER6', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 7"
                      value={this.state.features.FOREST_COVER7}
                      onChange={(e) => set('FOREST_COVER7', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 8"
                      value={this.state.features.FOREST_COVER8}
                      onChange={(e) => set('FOREST_COVER8', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 9"
                      value={this.state.features.FOREST_COVER9}
                      onChange={(e) => set('FOREST_COVER9', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 10"
                      value={this.state.features.FOREST_COVER10}
                      onChange={(e) => set('FOREST_COVER10', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 11"
                      value={this.state.features.FOREST_COVER11}
                      onChange={(e) => set('FOREST_COVER11', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 12"
                      value={this.state.features.FOREST_COVER12}
                      onChange={(e) => set('FOREST_COVER12', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 13"
                      value={this.state.features.FOREST_COVER13}
                      onChange={(e) => set('FOREST_COVER13', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 14"
                      value={this.state.features.FOREST_COVER14}
                      onChange={(e) => set('FOREST_COVER14', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 15"
                      value={this.state.features.FOREST_COVER15}
                      onChange={(e) => set('FOREST_COVER15', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 16"
                      value={this.state.features.FOREST_COVER16}
                      onChange={(e) => set('FOREST_COVER16', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 17"
                      value={this.state.features.FOREST_COVER17}
                      onChange={(e) => set('FOREST_COVER17', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 18"
                      value={this.state.features.FOREST_COVER18}
                      onChange={(e) => set('FOREST_COVER18', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 19"
                      value={this.state.features.FOREST_COVER19}
                      onChange={(e) => set('FOREST_COVER19', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 20"
                      value={this.state.features.FOREST_COVER20}
                      onChange={(e) => set('FOREST_COVER20', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 21"
                      value={this.state.features.FOREST_COVER21}
                      onChange={(e) => set('FOREST_COVER21', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 22"
                      value={this.state.features.FOREST_COVER22}
                      onChange={(e) => set('FOREST_COVER22', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 23"
                      value={this.state.features.FOREST_COVER23}
                      onChange={(e) => set('FOREST_COVER23', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 24"
                      value={this.state.features.FOREST_COVER24}
                      onChange={(e) => set('FOREST_COVER24', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 25"
                      value={this.state.features.FOREST_COVER25}
                      onChange={(e) => set('FOREST_COVER25', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 26"
                      value={this.state.features.FOREST_COVER26}
                      onChange={(e) => set('FOREST_COVER26', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 27"
                      value={this.state.features.FOREST_COVER27}
                      onChange={(e) => set('FOREST_COVER27', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 28"
                      value={this.state.features.FOREST_COVER28}
                      onChange={(e) => set('FOREST_COVER28', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 29"
                      value={this.state.features.FOREST_COVER29}
                      onChange={(e) => set('FOREST_COVER29', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 30"
                      value={this.state.features.FOREST_COVER30}
                      onChange={(e) => set('FOREST_COVER30', parseFloat(e))}
                    />
                    <TextField
                      type='number'
                      label="Forest Cover for Subbasin 31"
                      value={this.state.features.FOREST_COVER31}
                      onChange={(e) => set('FOREST_COVER31', parseFloat(e))}
                    />
                    <Button
                      onClick={submit}
                    >
                      Submit
                    </Button>
                  </FormLayout>
                </Card>
              </Layout.Section>
            </Layout>
          : result}
        </Page>
      </AppProvider>
    );
  }
}

export default App;
