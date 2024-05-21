export type Album = {
  id: string;
  title: string;
  photos: Photo[];
};

export type Photo = {
  id: string;
  url?: string;
  thumbnails?: Thumbnails;
};

export type Thumbnails = {
  small?: Thumbnail;
  medium?: Thumbnail;
  large?: Thumbnail;
};

export type Thumbnail = {
  url: string;
};

export type PhotoUpload = {
  id: string;
  url: string;
};
