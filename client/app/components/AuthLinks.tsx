import Link from "next/link"
import { useAuth } from "../contexts/AuthContext";
import { logoutUser } from "../utils/api";
import { useRouter } from "next/navigation";

export default function AuthLinks({ pathname }: { pathname: string }) {
  const { isAdmin, isLogged, setIsAdmin, setIsLogged } = useAuth();
  const router = useRouter();

  const handleLogout = async () => {
    try {
      await logoutUser();
      setIsAdmin(false);
      setIsLogged(false);
      localStorage.removeItem('token');
      localStorage.removeItem('userId');
      router.push('/');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };
  
  if (pathname === "/signin" || pathname === "/signup") {
    return null;
  }

  if (!isLogged) {
    return (
      <div>
        <Link
          href="/signin"
          className="text-sm px-4 py-2 rounded-lg border-2 border-teal-500 text-gray-500 hover:text-white hover:border-teal-600"
        >
          Fazer login
        </Link>
        <Link
          href="/signup"
          className="text-sm px-4 py-2 rounded-lg bg-teal-500 text-white hover:bg-teal-600 ml-2"
        >
          Registrar-se
        </Link>
      </div>
    );
  }

  const logoutLink = (
    <button
      onClick={handleLogout}
      className="ml-4 text-sm px-4 py-2 rounded-lg bg-teal-500 text-white hover:bg-teal-600"
    >
      Sair
    </button>
  );

  if (isAdmin) {
    if (pathname === "/") {
      return (
        <div>
          <Link
            href="/post/new"
            className="text-sm px-4 py-2 rounded-lg border-2 border-teal-500 text-gray-500 hover:text-white hover:border-teal-600"
          >
            New Post
          </Link>
          {logoutLink}
        </div>
      );
    } else if (pathname === "/projects") {
      return (
        <div>
          <Link
            href="/projects/new"
            className="text-sm px-4 py-2 rounded-lg border-2 border-teal-500 text-gray-500 hover:text-white hover:border-teal-600"
          >
            New Project
          </Link>
          {logoutLink}
        </div>
      );
    }
  }

  return logoutLink;
}