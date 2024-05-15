export type Album = {
  id: string;
  title: string;
  photos: Photo[];
};

export type Photo = {
  id: string;
  url?: string;
};

export type PhotoUpload = {
  id: string;
  url: string;
};
