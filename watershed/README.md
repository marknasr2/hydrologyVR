# watershed

Models downstream run-off of the [South Nation Watershed](http://www.agr.gc.ca/eng/?id=1298408711261) as based on subbasin forest cover and precipitation metrics.
You can find the raw outputs from the [Provincial (Stream) Water Quality Monitoring Network](https://www.ontario.ca/environment-and-energy/map-provincial-stream-water-quality-monitoring-network) under `Stream: South Nation River`.

Under Dr. Ousmane Seidou (uOttawa) and Matthew Noteboom (AgCanada).

## Installation

We package everything in Docker and Docker-Compose which installs on Windows, Mac, and Linux. Installation instructions for both are on the [Docker Website](https://docs.docker.com/compose/install/). We use Docker-Compose reference 3.0 which requires Docker Engine version >= `1.13.0`.

## Usage

```sh
docker-compose up
```

Then open your web browser to localhost:8000

## Model Description
The purpose of these models are to estimate the run-offs of various nutrients into the water stream, as a result of changes in environmental or land-use conditions in the watershed.
One of the novel approaches in this system is the separation of watershed characteristics into individual subbasins; we treat the subbasins as separate inputs with outputs measured at a downstream point.
This allows us to ignore the particular flow characteristics of the watershed (eg. how one subbasin flows into another subbasin) and instead use regression approaches to weight the subbasins according to their impacts on the downstream measurements.

The project should be considered as simply a proof-of-concept and could be significantly improved with more subbasin parameters.
We think this approach demonstrates a reliable reproduction of the same outputs from equation-based models such as those used by SWAT.
In the future, such methods can be used to simplify environmental modelling by only requiring recordings from different input sites instead of traditional surveying requirements.

## Implementation Description
The models are created using Random Forest Regression model from the Python Scikit-Learn package.
The initial data extraction and exploration were performed using Jupyter Notebooks, and the final API is developed in Flask with packaging in Docker.
Pipenv is used to manage the Python dependencies for this project.
This separates the model API into an individually packaged container, and we also provide a frontend developed in ReactJS for interacting with the model.

## Additional Details

### Running the Jupyter Notebooks

The code is written in Python 3.6 and the dependencies are managed using [Pipenv](https://docs.pipenv.org/).

Assuming you already have Python 3.6 installed, the instructions for installing Pipenv are available [here](https://docs.pipenv.org/install/#installing-pipenv).

Then install dependencies:

```sh
pipenv install
```

Then to install the Python kernel for Jupyter there's a bit of setup involved. First run:

```sh
pipenv shell
exit
```

which should give you a prompt with the name of your virtualenv, such as `watershed-EG5ajtUt`.
You'll need this name to add the Python kernel for Jupyter, like so (replacing `watershed-EG5ajtUt` with whatever your virtualenv name ends up being):

```sh
pipenv run python -m ipykernel install --user --name watershed-EG5ajtUt --display-name "watershed"
```

Then you can finally start the Jupyter Notebook via:

```sh
pipenv run jupyter notebook
```
