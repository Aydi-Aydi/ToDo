import { Route, Routes } from "react-router-dom"
import { Home } from "./Home"
import { Login } from "./Login"
import { Register} from "./Register"
import { Task } from "./Task"



export const Routing = () => {
    return <>
        <Routes>
            <Route path="/" element={<Home></Home>} ></Route>
            <Route path="home" element={<Home></Home>}></Route>
            <Route path="login" element={<Login></Login>}></Route>
            <Route path="register" element={<Register></Register>}></Route>
            <Route path ="task" element={<Task></Task>}></Route>
        </Routes>
    </>
}