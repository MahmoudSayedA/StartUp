import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
  vus: 10,
  duration: '30s',
};

export default function () {
  let res = http.get('https://test.k6.io');
  console.log('Response status: ' + res.status);
  sleep(5);
}