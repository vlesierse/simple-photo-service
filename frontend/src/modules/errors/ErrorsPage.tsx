import { Route, Routes } from "react-router-dom";
import { ErrorsLayout } from "./ErrorsLayout";
import { Error404 } from "./components/Error404";

const ErrorsPage = () => (
  <Routes>
    <Route element={<ErrorsLayout />}>
      <Route path="404" element={<Error404 />} />
      <Route index element={<Error404 />} />
    </Route>
  </Routes>
);

export { ErrorsPage };
