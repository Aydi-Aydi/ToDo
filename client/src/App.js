import { Nav } from './components/nav';
import { BrowserRouter } from 'react-router-dom';
import { Routing } from './components/routing';

function App() {
  return(
  <div className="App">
  <BrowserRouter>
<Nav></Nav>
<Routing></Routing>
</BrowserRouter>
</div>
  );
}

export default App;
