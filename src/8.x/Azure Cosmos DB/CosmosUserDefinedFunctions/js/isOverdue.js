function isOverdue(item) {
    if (!item.Reminder) {
        return false;
    }

    var reminderDate = new Date(item.Reminder);
    var currentDate = new Date();

    return currentDate > reminderDate && !item.Done;
}