export const parseUrl = (path: string, base: string) => {
    if (path.startsWith('/')) {
        path = path.substring(1);
    }

    if (base.endsWith('/')) {
        base = base.substring(0, base.length - 2);
    }

    return base + '/' + path;
}