/** Bỏ dấu tiếng Việt + đ/Đ để so khớp không phân biệt dấu. */
export function removeDiacritics(value: string): string {
  return value
    .toLowerCase()
    .normalize("NFD")
    .replace(/[̀-ͯ]/g, "")
    .replace(/đ/g, "d");
}
