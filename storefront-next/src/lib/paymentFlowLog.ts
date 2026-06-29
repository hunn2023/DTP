const LOG_PREFIX = "[PaymentFlow]";

type LogLevel = "info" | "warn" | "error";

function formatError(error: unknown) {
  if (error instanceof Error) {
    return {
      name: error.name,
      message: error.message,
      stack: error.stack,
    };
  }

  return { message: String(error) };
}

export function paymentFlowLog(
  step: string,
  data?: Record<string, unknown>,
  level: LogLevel = "info"
) {
  const payload = {
    ts: new Date().toISOString(),
    step,
    ...data,
  };

  const message = `${LOG_PREFIX} ${step}`;

  if (level === "error") {
    console.error(message, payload);
    return;
  }

  if (level === "warn") {
    console.warn(message, payload);
    return;
  }

  console.info(message, payload);
}

export function paymentFlowError(step: string, error: unknown, data?: Record<string, unknown>) {
  paymentFlowLog(step, { ...data, error: formatError(error) }, "error");
}
