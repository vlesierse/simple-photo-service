import { Route, Routes } from "react-router-dom";

import { MyAlbums } from "./MyAlbums";
import { CreateAlbum } from "./CreateAlbum";

export const AlbumsPage = () => {
  return (
    <Routes>
      <Route path="create" element={<CreateAlbum />} />
      <Route index element={<MyAlbums />} />
    </Routes>
  );
};
