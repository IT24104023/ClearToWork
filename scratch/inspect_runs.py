import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

runs = re.findall(r'href="/IT24104023/ClearToWork/actions/runs/(\d+)"[^>]*>([^<]+)</a>', html)
for r_id, title in runs[:6]:
    print(f"Run {r_id}: {title.strip()}")
