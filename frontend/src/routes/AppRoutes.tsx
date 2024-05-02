import { Navigate, Route, BrowserRouter as Router, Routes } from "react-router-dom";

import { App } from "../App";
import { PrivateRoutes } from "./PrivateRoutes";

import { ErrorsPage } from "../modules/errors/ErrorsPage";
import { Logout } from "../modules/auth/Logout";
import { AuthPage } from "../modules/auth/AuthPage";
import { useAuth } from "../modules/auth/core";

const AppRoutes = () => {
  const { currentUser } = useAuth();
  return (
    <Router>
      <Routes>
        <Route element={<App />}>
          <Route path="error/*" element={<ErrorsPage />} />
          <Route path="logout" element={<Logout />} />
          {currentUser ? (
            <>
              <Route path="auth/*" element={<Navigate to="/" />} />
              <Route path="/*" element={<PrivateRoutes />} />
              <Route index element={<Navigate to="/albums" />} />
            </>
          ) : (
            <>
              <Route path="auth/*" element={<AuthPage />} />
              <Route path="*" element={<Navigate to="/auth" />} />
            </>
          )}
        </Route>
      </Routes>
    </Router>
  );
};

export { AppRoutes };
