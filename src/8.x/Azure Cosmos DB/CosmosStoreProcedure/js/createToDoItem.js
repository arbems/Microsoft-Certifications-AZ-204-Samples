function createToDoItem(item) {
    var context = getContext();
    var collection = context.getCollection();
    var accepted = collection.createDocument(
        collection.getSelfLink(),
        item,
        function (err, newitem) {
            if (err) throw new Error('Error' + err.message);
            context.getResponse().setBody(newitem);
        }
    );
    if (!accepted) return;
}