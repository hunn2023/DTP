/** null = lưu thành công, string = thông báo lỗi hiển thị cho user. */
export type WizardSaveFn = () => Promise<string | null>

export type WizardSaveOutcome =
  | { ok: true }
  | { ok: false; message: string }

export function toWizardSaveOutcome(errorMessage: string | null | undefined): WizardSaveOutcome {
  if (errorMessage) return { ok: false, message: errorMessage }
  return { ok: true }
}
