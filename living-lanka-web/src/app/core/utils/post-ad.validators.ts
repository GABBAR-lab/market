const SL_MOBILE_REGEX = /^(0[1-9]\d{8}|[1-9]\d{8})$/;

export function normalizeSlDigits(phone: string): string {
  let digits = phone.replace(/\D/g, '');
  if (digits.startsWith('94')) {
    digits = digits.slice(2);
  }
  if (digits.startsWith('0')) {
    digits = digits.slice(1);
  }
  return digits;
}

export function isValidSlMobile(phone: string): boolean {
  const digits = normalizeSlDigits(phone);
  return SL_MOBILE_REGEX.test(`0${digits}`) || digits.length === 9;
}

export function formatSlMobileDisplay(phone: string): string {
  const d = normalizeSlDigits(phone);
  return d.length === 9 ? `0${d}` : phone;
}

export function validateAdTitle(title: string): string | null {
  const t = title.trim();
  if (!t) return 'Advertisement title is required.';
  if (t.length < 5) return 'Title must be at least 5 characters.';
  if (t.length > 150) return 'Title must be at most 150 characters.';
  return null;
}

export function validateDescription(desc: string): string | null {
  const d = desc.trim();
  if (!d) return 'Description is required.';
  if (d.length < 20) return 'Description must be at least 20 characters.';
  return null;
}

export function validateImageCount(count: number, min = 4, max = 10): string | null {
  if (count < min) return `Please upload at least ${min} photos.`;
  if (count > max) return `Maximum ${max} photos allowed.`;
  return null;
}

export function validateCardNumber(num: string): string | null {
  const digits = num.replace(/\D/g, '');
  if (digits.length < 13 || digits.length > 19) return 'Invalid card number.';
  return null;
}

export function validateCvv(cvv: string): string | null {
  if (!/^\d{3,4}$/.test(cvv)) return 'CVV must be 3 or 4 digits.';
  return null;
}
