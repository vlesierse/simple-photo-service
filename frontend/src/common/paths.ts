export const AppRoutes = {
  Home: "/",
  Albums: {
    Home: "/albums",
    View: "/albums/:id",
  },
};

export const noHash = (path: string) => {
  return path.substring(2);
};

/**
 * Inserts path parameres into the specified route.
 * @param route the route to insert params into
 * @param params the parameters to insert
 * @returns The complete URI with path parameters.
 */
export const buildPathParams = (
  route: string,
  params?: { [key: string]: string | undefined }
) => {
  return params
    ? Object.keys(params).reduce((acc, key) => {
        return acc.replace(":" + key, encodeURIComponent(params[key] ?? ""));
      }, route)
    : route;
};

/**
 * Builds query parameters for the specified route.
 * TODO: add support for multivariate params
 * @param route the route to build params for
 * @param params the parameters to add
 * @returns The complete URI with query parameters.
 */
export const buildQueryParams = (
  route: string,
  params?: { [key: string]: string | string[] | number | boolean | undefined }
) => {
  return params
    ? `${route}?${Object.keys(params)
        .filter((key) => params[key] !== undefined)
        .map((key) => {
          const param = params[key];
          return param instanceof Array
            ? param
                .map((p) => key + "=" + encodeURIComponent(p ?? ""))
                .join("&")
            : key + "=" + encodeURIComponent(param ?? "");
        })
        .join("&")}`
    : route;
};

export const asLink = (
  route: string,
  queryParams?: {
    [key: string]: string | string[] | number | boolean | undefined;
  }
) => {
  return buildQueryParams(route, queryParams);
};
