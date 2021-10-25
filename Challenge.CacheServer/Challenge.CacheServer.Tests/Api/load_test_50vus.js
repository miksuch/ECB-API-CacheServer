import http from 'k6/http';
import {setup as baselineSetup} from './baseline_test.js';

export let options = {
    vus: 50,
    duration: "10s",
}

export function setup(){
    return baselineSetup();
}

export default function (data) {
    http.get(data.query);
}