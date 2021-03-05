import * as React from 'react';
import { Route } from 'react-router';
import Project from './components/Project';

import './custom.css'

export default () => (
    //<Layout>
        <Route path='/' component={Project} />
    //</Layout>
);
