import axios from 'axios';


const apiUrl = process.env.REACT_APP_API
axios.defaults.baseURL = process.env.REACT_APP_API_URL;
setAuthorizationBearer();

function saveAccessToken(authResult) {
  localStorage.setItem("access_token", authResult.token);
  setAuthorizationBearer();
}
function setAuthorizationBearer() {
  const accessToken = localStorage.getItem("access_token");
  if (accessToken) {
    axios.defaults.headers.common["Authorization"] = `Bearer ${accessToken}`;
  }
}
axios.interceptors.response.use(
  function(response) {
    return response;
  },
  function(error) {
    if (error.response.status === 401) {
      return (window.location.href = "/login");
    }
    return Promise.reject(error);
  }
);
export default {

  getTasks: async () => {
    const result = await axios.get(`${apiUrl}/tasks`);    
    return result.data;
  },

  addTask: async (name) => {
    console.log('addTask', name);
    console.log('Sending data:', { name: name });
    try {
      const result = await axios.post(`${apiUrl}/tasks`, { name: name }, { headers: { "Content-Type": "application/json" } });
      return result;
    } catch (error) {
      console.error('Error adding task:', error);
      throw error; 
    }
  },
  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete });
    const result = await axios.put(`${apiUrl}/tasks/${id}`, isComplete, {
        headers: { "Content-Type": "application/json" } 
    });
    return result;
},
login: async (UserName, password) => {
  console.log("Sending login request with:", { UserName, password });
  try {
      const res = await axios.post(`${apiUrl}/login`, { userName: UserName, Password: password });
      console.log("Login response:", res.data);
      saveAccessToken(res.data);
  } catch (error) {
      console.error("Error during login request:", error);
      throw error;
  }
},


  deleteTask:async(id)=>{
    console.log('deleteTask')
    const result = await axios.delete(`${apiUrl}/tasks/${id}`); 
    return result; 

  },
  register: async (UserName, password) => {
    const res = await axios.post(`${apiUrl}/register`, { UserName, password });
    saveAccessToken(res.data);
  },
};
