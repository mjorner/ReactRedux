const requestWeatherForecastsType = 'REQUEST_WEATHER_FORECASTS';
const receiveWeatherForecastsType = 'RECEIVE_WEATHER_FORECASTS';
const initialState = { forecasts: [], isLoading: false };

export const actionCreators = {
  requestWeatherForecasts: filename => async (dispatch, getState) => {    
    if (filename === getState().weatherForecasts.filename) {
      // Don't issue a duplicate request (we already have or are loading the requested data)
      return;
    }

    dispatch({ type: requestWeatherForecastsType, filename });

    const url = `api/Data/ReadGraphData?filename=${filename}`;
    const response = await fetch(url);
    const forecasts = await response.json();

    dispatch({ type: receiveWeatherForecastsType, filename, forecasts });
  }
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === requestWeatherForecastsType) {
    return {
      ...state,
      filename: action.filename,
      isLoading: true
    };
  }

  if (action.type === receiveWeatherForecastsType) {
    return {
      ...state,
      filename: action.filename,
      forecasts: action.forecasts,
      isLoading: false
    };
  }

  return state;
};
