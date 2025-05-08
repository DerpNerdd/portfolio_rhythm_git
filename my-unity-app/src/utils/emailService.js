import emailjs from '@emailjs/browser';

const PUBLIC_KEY  = process.env.REACT_APP_EMAILJS_PUBLIC_KEY;
const SERVICE_ID  = process.env.REACT_APP_EMAILJS_SERVICE_ID;
const TEMPLATE_ID = process.env.REACT_APP_EMAILJS_TEMPLATE_ID;

export function initEmailJS() {
  console.log('🔑 PUBLIC_KEY:', PUBLIC_KEY);
  emailjs.init(PUBLIC_KEY);
}

export function sendEmailForm(formElement) {
  return emailjs.sendForm(SERVICE_ID, TEMPLATE_ID, formElement);
}
