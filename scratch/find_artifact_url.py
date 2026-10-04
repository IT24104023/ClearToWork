import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/37244321284"
html = urllib.request.urlopen(urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})).read().decode('utf-8')

matches = re.findall(r'<a[^>]+href="([^"]+)"[^>]*>.*?cleartowork.*?</a>', html, re.IGNORECASE | re.DOTALL)
print("Matching links:", matches)

if not matches:
    # Print lines containing artifact or download
    for line in html.splitlines():
        if "artifact" in line.lower() or "release" in line.lower() or "apk" in line.lower():
            if "href" in line:
                print("Line:", line.strip())
