import { Navigate } from "react-router-dom";

type Props = { children: any };

export default function ProtectedRoute({ children }: Props) {
  const token = localStorage.getItem("jwt");
  if (!token) {
    return <Navigate to="/login" replace />;
  }
  return children;
}