import Link from 'next/link'
import { ThemeContext } from './MyContext.js'

const linkStyle = {
}

/*
	{<Link href="/">
		<a className="navbar-item" style={linkStyle}>Home</a>
</Link>}*/
export const NavBar = (props) => (
    <ThemeContext.Consumer>
      {context => (
		<div className="navbar-start">
			{context.loggedIn && (
				<span/>
			)}
			<Link href="/about">
				<a className="navbar-item" style={linkStyle}>About</a>
			</Link>

			{!context.loggedIn && (
					<Link href="/login">
						<a className="navbar-item" style={linkStyle}>Login</a>
					</Link>
			)}
			{!context.loggedIn && (
					<Link href="/register">
						<a className="navbar-item" style={linkStyle}>Register</a>
					</Link>
			)}
			{context.loggedIn && (
					<Link href="/orders">
						<a className="navbar-item" style={linkStyle}>My Orders</a>
					</Link>
			)}
		
		</div>
      )}
    </ThemeContext.Consumer>
)
//{context.loggedIn ? (<p>Logged in.</p>) : (<p>Not Logged in.</p>)}


export const Header = () => (
    <div>
    
	<nav className="navbar is-link">
		<div className="navbar-brand">
            <Link href="/">
                <a className="navbar-item"><i className="fa fa-shopping-bag"></i></a>
            </Link>
            <NavBar/>
			<div className="navbar-burger burger" data-target="navMenuColordark">
				<span></span><span></span><span></span>
			</div>
		</div>
		<div className="navbar-menu" id="navMenuColordark">
		
			<div className="navbar-end">
				<div className="navbar-item">
					<div className="field is-grouped">
						<p className="control">
						
						<Link href="/">
								<a className="bd-tw-button button"><span className="icon"><i className="fa fa-home"></i></span><span>Home</span></a>
							</Link>
							</p>
						</div>
				</div>
			</div>
		</div>
	</nav>
    </div>
)

//export default Header