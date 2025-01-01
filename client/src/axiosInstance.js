import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'http://localhost:5028/api',
  withCredentials: true
});

export default axiosInstance;
