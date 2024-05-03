import { Route, Routes } from "react-router-dom";

import { MyAlbums } from "./MyAlbums";
import { ViewAlbum } from "./ViewAlbum";
import { CreateAlbum } from "./CreateAlbum";

export const AlbumsPage = () => {
  return (
    <Routes>
      <Route path="create" element={<CreateAlbum />} />
      <Route path=":id" element={<ViewAlbum />} />
      <Route index element={<MyAlbums />} />
    </Routes>
  );
};
