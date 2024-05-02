import { Route, Routes } from "react-router-dom";

import { PortalLayout } from "../components/layouts/PortalLayout";
import { AlbumsPage } from "../modules/albums/AlbumsPage";

const PrivateRoutes = () => {
  return (
    <Routes>
      <Route element={<PortalLayout />}>
        <Route path="albums/*" element={<AlbumsPage />} />
      </Route>
    </Routes>
  );
};

export { PrivateRoutes };
