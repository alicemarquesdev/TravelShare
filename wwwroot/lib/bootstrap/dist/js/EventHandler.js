import { addHandler, normalizeParams, getEvent, removeHandler, removeNamespacedHandlers, stripUidRegex, getjQuery, getTypeEvent, nativeEvents } from './bootstrap.esm';

export const EventHandler = {
    on(element, event, handler, delegationFn) {
        addHandler(element, event, handler, delegationFn, false);
    },

    one(element, event, handler, delegationFn) {
        addHandler(element, event, handler, delegationFn, true);
    },

    off(element, originalTypeEvent, handler, delegationFn) {
        if (typeof originalTypeEvent !== 'string' || !element) {
            return;
        }

        const [delegation, originalHandler, typeEvent] = normalizeParams(originalTypeEvent, handler, delegationFn);
        const inNamespace = typeEvent !== originalTypeEvent;
        const events = getEvent(element);
        const isNamespace = originalTypeEvent.startsWith('.');

        if (typeof originalHandler !== 'undefined') {
            // Simplest case: handler is passed, remove that listener ONLY.
            if (!events || !events[typeEvent]) {
                return;
            }

            removeHandler(element, events, typeEvent, originalHandler, delegation ? handler : null);
            return;
        }

        if (isNamespace) {
            Object.keys(events).forEach(elementEvent => {
                removeNamespacedHandlers(element, events, elementEvent, originalTypeEvent.slice(1));
            });
        }

        const storeElementEvent = events[typeEvent] || {};
        Object.keys(storeElementEvent).forEach(keyHandlers => {
            const handlerKey = keyHandlers.replace(stripUidRegex, '');

            if (!inNamespace || originalTypeEvent.includes(handlerKey)) {
                const event = storeElementEvent[keyHandlers];
                removeHandler(element, events, typeEvent, event.originalHandler, event.delegationSelector);
            }
        });
    },

    trigger(element, event, args) {
        if (typeof event !== 'string' || !element) {
            return null;
        }

        const $ = getjQuery();
        const typeEvent = getTypeEvent(event);
        const inNamespace = event !== typeEvent;
        const isNative = nativeEvents.has(typeEvent);
        let jQueryEvent;
        let bubbles = true;
        let nativeDispatch = true;
        let defaultPrevented = false;
        let evt = null;

        if (inNamespace && $) {
            jQueryEvent = $.Event(event, args);
            $(element).trigger(jQueryEvent);
            bubbles = !jQueryEvent.isPropagationStopped();
            nativeDispatch = !jQueryEvent.isImmediatePropagationStopped();
            defaultPrevented = jQueryEvent.isDefaultPrevented();
        }

        if (isNative) {
            evt = document.createEvent('HTMLEvents');
            evt.initEvent(typeEvent, bubbles, true);
        } else {
            evt = new CustomEvent(event, {
                bubbles,
                cancelable: true
            });
        } // merge custom information in our event

        if (typeof args !== 'undefined') {
            Object.keys(args).forEach(key => {
                Object.defineProperty(evt, key, {
                    get() {
                        return args[key];
                    }
                });
            });
        }

        if (defaultPrevented) {
            evt.preventDefault();
        }

        if (nativeDispatch) {
            element.dispatchEvent(evt);
        }

        if (evt.defaultPrevented && typeof jQueryEvent !== 'undefined') {
            jQueryEvent.preventDefault();
        }

        return evt;
    }
};