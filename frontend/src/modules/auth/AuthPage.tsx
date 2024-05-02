import { Route, Routes } from "react-router-dom";

import { SignIn } from "./components/SignIn";
import { SignUp } from "./components/SignUp";
import { ConfirmSignUp } from "./components/ConfirmSignUp";
import { DefaultLayout } from "../../components/layouts/DefaultLayout";

const AuthPage = () => (
  <Routes>
    <Route element={<DefaultLayout />}>
      <Route path="signin" element={<SignIn />} />
      <Route path="signup" element={<SignUp />} />
      <Route path="signup/confirm" element={<ConfirmSignUp />} />
      <Route index element={<SignIn />} />
    </Route>
  </Routes>
);

export { AuthPage };
