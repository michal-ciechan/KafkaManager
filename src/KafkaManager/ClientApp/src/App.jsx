import React, { Component } from 'react';
import { Route } from 'react-router';
import { AppLayout } from './components/AppLayout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';

import 'antd/dist/antd.css';
import 'antd/dist/antd.dark.css';


import './custom.css'
import {Topics} from "./components/Topics";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <AppLayout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <Route path='/fetch-data' component={FetchData} />
        <Route path='/topics' component={Topics} />
      </AppLayout>
    );
  }
}
