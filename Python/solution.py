def find_most_expensive(items, n=5, category="dvd"):
    # 1. What are the 5 most expensive items from each category?
    items = filter(lambda x: x.get("type", "") == category, items)
    # FIXME Instead of sorting in ~O(nlogn), it would be wiser to iterate manually in O(n)
    items.sort(key=lambda x: -x.get("price", 0))
    return items[:5]

def find_cds_by_duration(items, min_duration_in_seconds=60*60):
    # 2. Which cds have a total running time longer than 60 minutes?
    items = filter(lambda x: x.get("type", "") == "cd", items)
    # TODO This should be refactored into a filter
    valid_items = []
    for cd in items:
        duration = 0
        for track in cd["tracks"]:
            duration += track["seconds"]
        if duration >= min_duration_in_seconds:
            valid_items.append(cd)
    return valid_items

def find_cd_authors(items):
    # 3. Which authors have also released cds?
    # We have to iterate twice since the items order is random
    books = filter(lambda x: x.get("type", "") == "book", items)
    cds = filter(lambda x: x.get("type", "") == "cd", items)

    book_authors = set(map(lambda x: x["author"], books))
    cd_authors = set()

    for cd in cds:
        author = cd["author"]
        if author in book_authors:
            cd_authors.add(author)

    return list(cd_authors)

def _contains_year(s):
    numbers = [int(si) for si in s.split() if si.isdigit()]
    for number in numbers:
        if number <= 3000 and number >= 1000:
            return True
    return False

def find_yearitems(items):
    # 4. Which items have a title, track, or chapter that contains a year.
    items_with_year = []
    # TODO This should be a filter instead
    for item in items:
        data = []
        data.append(item.get("title", ""))
        data.append(item.get("track", {}).get("name", ""))
        data.extend(item.get("chapters", []))
        if _contains_year("".join(data)):
            items_with_year.append(item)
    return items_with_year


if __name__ == '__main__':
    stdin = """
[

{
    "price": 15.99,
    "chapters": [
        "one",
        "two",
        "three"
    ],
    "year": 1999,
    "title": "foo",
    "author": "mary",
    "type": "book"
},
{
    "price": 11.99,
    "minutes": 90,
    "year": 2004,
    "title": "bar",
    "director": "alan",
    "type": "dvd"
},
{
    "price": 15.99,
     "tracks": [
     {
     "seconds": 180,
     "name": "one"
     },
     {
     "seconds": 200,
     "name": "two"
     }
     ],
     "year": 2000,
     "title": "baz 1969",
     "author": "mary",
     "type": "cd"
}
]"""

    import json
    items = json.loads(stdin)
    assert len(find_most_expensive(items, "dvd")) == 1
    assert len(find_cds_by_duration(items, 180+200)) == 1
    assert len(find_cds_by_duration(items, 180+200+1)) == 0
    assert len(find_cd_authors(items)) == 1
    assert len(find_yearitems(items)) == 1