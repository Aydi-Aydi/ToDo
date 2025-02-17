import { Link } from "react-router-dom"

export const Nav = () => {
   return <>
      <nav className="navbar navbar-inverse" style={{width:"1400px"}} >
         <div className="container-fluid">
            <ul className="nav navbar-nav navbar-right" >
               <li><Link to={"login"} style={{color:"white"}}>התחברות</Link></li>
               <li><Link to={"register"}  style={{color:"white"}}>הרשמה</Link></li>
               <li><Link to={"home"} className="navbar-brand"  style={{color:"white"}}>בית</Link></li>
               <li><Link to={"task"}style={{color:"white"}}>משימות</Link></li>
            </ul>
         </div>
      </nav>
   </>
}